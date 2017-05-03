using MARC.HI.EHRS.SVC.Auditing.Data;
using MARC.HI.EHRS.SVC.Auditing.Services;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Security.Audit
{
    /// <summary>
    /// Event type codes
    /// </summary>
    public enum EventTypeCodes
    {
        [XmlEnum("SecurityAuditCode-ApplicationActivity")]
        ApplicationActtivity,
        [XmlEnum("SecurityAuditCode-AuditLogUsed")]
        AuditLogUsed,
        [XmlEnum("SecurityAuditCode-Export")]
        Export,
        [XmlEnum("SecurityAuditCode-Import")]
        Import,
        [XmlEnum("SecurityAuditCode-NetworkActivity")]
        NetworkActivity,
        [XmlEnum("SecurityAuditCode-OrderRecord")]
        OrderRecord,
        [XmlEnum("SecurityAuditCode-PatientRecord")]
        PatientRecord,
        [XmlEnum("SecurityAuditCode-ProcedureRecord")]
        ProcedureRecord,
        [XmlEnum("SecurityAuditCode-Query")]
        Query,
        [XmlEnum("SecurityAuditCode-SecurityAlert")]
        SecurityAlert,
        [XmlEnum("SecurityAuditCode-UserAuthentication")]
        UserAuthentication,
        [XmlEnum("SecurityAuditCode-ApplicationStart")]
        ApplicationStart,
        [XmlEnum("SecurityAuditCode-ApplicationStop")]
        ApplicationStop,
        [XmlEnum("SecurityAuditCode-Login")]
        Login,
        [XmlEnum("SecurityAuditCode-Logout")]
        Logout,
        [XmlEnum("SecurityAuditCode-Attach")]
        Attach,
        [XmlEnum("SecurityAuditCode-Detach")]
        Detach,
        [XmlEnum("SecurityAuditCode-NodeAuthentication")]
        NodeAuthentication,
        [XmlEnum("SecurityAuditCode-EmergencyOverrideStarted")]
        EmergencyOverrideStarted,
        [XmlEnum("SecurityAuditCode-Useofarestrictedfunction")]
        UseOfARestrictedFunction,
        [XmlEnum("SecurityAuditCode-Securityattributeschanged")]
        SecurityAttributesChanged,
        [XmlEnum("SecurityAuditCode-Securityroleschanged")]
        SecurityRolesChanged,
        [XmlEnum("SecurityAuditCode-Usersecurityattributechanged")]
        UserSecurityChanged
    }

    /// <summary>
    /// Security utility
    /// </summary>
    public static class AuditUtil
    {

        /// <summary>
        /// Audit that the audit log was used
        /// </summary>
        /// <param name="action"></param>
        /// <param name="outcome"></param>
        /// <param name="query"></param>
        /// <param name="auditIds"></param>
        public static void AuditAuditLogUsed(ActionType action, OutcomeIndicator outcome, String query, params Guid[] auditIds)
        {
            AuditData audit = new AuditData(DateTime.Now, action, outcome, EventIdentifierType.SecurityAlert, CreateAuditActionCode(EventTypeCodes.AuditLogUsed));

            // User actors
            AddDeviceActor(audit);
            AddUserActor(audit);
            AddSenderDeviceActor(audit);
            // Add objects to which the thing was done
            audit.AuditableObjects = auditIds.Select(o => new AuditableObject()
            {
                IDTypeCode = AuditableObjectIdType.ReportNumber,
                LifecycleType = action == ActionType.Delete ? AuditableObjectLifecycle.PermanentErasure : AuditableObjectLifecycle.Disclosure,
                ObjectId = o.ToString(),
                Role = AuditableObjectRole.SecurityResource,
                Type = AuditableObjectType.SystemObject
            }).ToList();

            if (!String.IsNullOrEmpty(query))
            {
                audit.AuditableObjects.Add(new AuditableObject()
                {
                    IDTypeCode = AuditableObjectIdType.SearchCritereon,
                    LifecycleType = AuditableObjectLifecycle.Access,
                    QueryData = query,
                    Role = AuditableObjectRole.Query,
                    Type = AuditableObjectType.SystemObject
                });
            }

            SendAudit(audit);

        }

        /// <summary>
        /// Autility utility which can be used to send a data audit 
        /// </summary>
        public static void AuditDataAction<TData>(EventTypeCodes typeCode, ActionType action, AuditableObjectLifecycle lifecycle, EventIdentifierType eventType, OutcomeIndicator outcome, String queryPerformed, params TData[] data) where TData : IdentifiedData
        {
            AuditCode eventTypeId = CreateAuditActionCode(typeCode);
            AuditData audit = new AuditData(DateTime.Now, action, outcome, eventType, eventTypeId);

            AddDeviceActor(audit);
            AddUserActor(audit);
            AddSenderDeviceActor(audit);

            // Objects
            audit.AuditableObjects = data.Select(o =>
            {

                var idTypeCode = AuditableObjectIdType.Custom;
                var roleCode = AuditableObjectRole.Resource;
                var objType = AuditableObjectType.Other;

                if (o is Patient)
                {
                    idTypeCode = AuditableObjectIdType.PatientNumber;
                    roleCode = AuditableObjectRole.Patient;
                    objType = AuditableObjectType.Person;
                }
                else if (o is UserEntity || o is Provider)
                {
                    idTypeCode = AuditableObjectIdType.UserIdentifier;
                    objType = AuditableObjectType.Person;
                    roleCode = AuditableObjectRole.Provider;
                }
                else if (o is Entity)
                    idTypeCode = AuditableObjectIdType.EnrolleeNumber;
                else if (o is Act)
                {
                    idTypeCode = AuditableObjectIdType.EncounterNumber;
                    roleCode = AuditableObjectRole.Report;
                }
                else if (o is SecurityUser)
                {
                    idTypeCode = AuditableObjectIdType.UserIdentifier;
                    roleCode = AuditableObjectRole.SecurityUser;
                    objType = AuditableObjectType.SystemObject;
                }

                return new AuditableObject()
                {
                    IDTypeCode = idTypeCode,
                    CustomIdTypeCode = idTypeCode == AuditableObjectIdType.Custom ? new AuditCode(o.GetType().Name, "OpenIZTable") : null,
                    LifecycleType = lifecycle,
                    ObjectId = o.Key?.ToString(),
                    Role = roleCode,
                    Type = objType
                };
            }).ToList();

            // Query performed
            if (!String.IsNullOrEmpty(queryPerformed))
            {
                audit.AuditableObjects.Add(new AuditableObject()
                {
                    IDTypeCode = AuditableObjectIdType.SearchCritereon,
                    LifecycleType = AuditableObjectLifecycle.Access,
                    QueryData = queryPerformed,
                    Role = AuditableObjectRole.Query,
                    Type = AuditableObjectType.SystemObject
                });
            }

            SendAudit(audit);
        }

        /// <summary>
        /// Create a security attribute action audit
        /// </summary>
        public static void AuditSecurityAttributeAction(IEnumerable<Object> objects, bool success, IEnumerable<string> changedProperties)
        {
            var audit = new AuditData(DateTime.Now, ActionType.Update, success ? OutcomeIndicator.Success : OutcomeIndicator.EpicFail, EventIdentifierType.SecurityAlert, CreateAuditActionCode(EventTypeCodes.SecurityAttributesChanged));
            AddDeviceActor(audit);
            AddUserActor(audit);
            AddSenderDeviceActor(audit);

            audit.AuditableObjects = objects.Select(obj => new AuditableObject()
            {
                IDTypeCode = AuditableObjectIdType.Custom,
                CustomIdTypeCode = new AuditCode(obj.GetType().Name, "OpenIZTable"),
                ObjectId = ((obj as IIdentifiedEntity)?.Key ?? Guid.Empty).ToString(),
                LifecycleType = AuditableObjectLifecycle.Amendment,
                ObjectData = changedProperties.Select(
                    kv => new ObjectDataExtension(
                        kv.Contains("=") ? kv.Substring(0, kv.IndexOf("=")) : kv,
                        kv.Contains("=") ? Encoding.UTF8.GetBytes(kv.Substring(kv.IndexOf("=") + 1)) : new byte[0]
                    )
                ).ToList(),
                Role = AuditableObjectRole.SecurityResource,
                Type = AuditableObjectType.SystemObject
            }).ToList();
            SendAudit(audit);
        }

        /// <summary>
        /// Send specified audit
        /// </summary>
        internal static void SendAudit(AuditData audit)
        {
            ApplicationContext.Current.GetService<IAuditorService>()?.SendAudit(audit);
        }

        /// <summary>
        /// Add user actor
        /// </summary>
        internal static void AddUserActor(AuditData audit)
        {
            var configService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

            // For the user
            audit.Actors.Add(new AuditActorData()
            {
                NetworkAccessPointId = Dns.GetHostName(),
                NetworkAccessPointType = NetworkAccessPointType.MachineName,
                UserName = AuthenticationContext.Current.Principal.Identity.Name,
                UserIdentifier = configService.GetUser(AuthenticationContext.Current.Principal.Identity).Key.ToString(),
                UserIsRequestor = true
            });
        }

        /// <summary>
        /// Add device actor
        /// </summary>
        internal static void AddDeviceActor(AuditData audit)
        {

            // For the current device name
            audit.Actors.Add(new AuditActorData()
            {
                NetworkAccessPointId = Dns.GetHostName(),
                NetworkAccessPointType = NetworkAccessPointType.MachineName,
                UserName = Environment.MachineName,
                ActorRoleCode = new List<AuditCode>() {
                    new  AuditCode("110152", "DCM") { DisplayName = "Destination" }
                }
            });

        }

        /// <summary>
        /// Add device actor
        /// </summary>
        internal static void AddSenderDeviceActor(AuditData audit)
        {
            var remoteAddress = (OperationContext.Current?.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty)?.Address;
            if (remoteAddress == null) return;

            // For the current device name
            audit.Actors.Add(new AuditActorData()
            {
                NetworkAccessPointId = remoteAddress,
                NetworkAccessPointType = NetworkAccessPointType.IPAddress,
                ActorRoleCode = new List<AuditCode>() {
                    new  AuditCode("110153", "DCM") { DisplayName = "Source" }
                }
            });

        }

        /// <summary>
        /// Create audit action code
        /// </summary>
        internal static AuditCode CreateAuditActionCode(EventTypeCodes typeCode)
        {
            var typeCodeWire = typeof(EventTypeCodes).GetRuntimeField(typeCode.ToString()).GetCustomAttribute<XmlEnumAttribute>();
            return new AuditCode(typeCodeWire.Name, "SecurityAuditCode");
        }

        /// <summary>
        /// Audit application start or stop
        /// </summary>
        public static void AuditApplicationStartStop(EventTypeCodes eventType)
        {
            AuditData audit = new AuditData(DateTime.Now, ActionType.Execute, OutcomeIndicator.Success, EventIdentifierType.ApplicationActivity, CreateAuditActionCode(eventType));
            AddDeviceActor(audit);
            SendAudit(audit);
        }

        /// <summary>
        /// Audit a login of a principal
        /// </summary>
        public static void AuditLogin(IPrincipal principal, String identityName, IIdentityProviderService identityProvider, bool successfulLogin = true)
        {
            AuditData audit = new AuditData(DateTime.Now, ActionType.Execute, successfulLogin ? OutcomeIndicator.Success : OutcomeIndicator.EpicFail, EventIdentifierType.UserAuthentication, CreateAuditActionCode(EventTypeCodes.Login));
            audit.Actors.Add(new AuditActorData()
            {
                NetworkAccessPointType = NetworkAccessPointType.MachineName,
                NetworkAccessPointId = Dns.GetHostName(),
                UserName = principal?.Identity?.Name ?? identityName,
                UserIsRequestor = true,
                ActorRoleCode = principal == null ? null : ApplicationContext.Current.GetService<IRoleProviderService>()?.GetAllRoles(principal.Identity.Name).Select(o =>
                    new AuditCode(o, null)
                ).ToList()
            });
            AddDeviceActor(audit);
            AddSenderDeviceActor(audit);

            audit.AuditableObjects.Add(new AuditableObject()
            {
                IDTypeCode = AuditableObjectIdType.Uri,
                NameData = identityProvider.GetType().AssemblyQualifiedName,
                ObjectId = $"http://openiz.org/auth/{identityProvider.GetType().FullName.Replace(".", "/")}",
                Type = AuditableObjectType.SystemObject,
                Role = AuditableObjectRole.Job
            });

            SendAudit(audit);
        }

        /// <summary>
        /// Audit a login of a principal
        /// </summary>
        public static void AuditLogout(IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            AuditData audit = new AuditData(DateTime.Now, ActionType.Execute, OutcomeIndicator.Success, EventIdentifierType.UserAuthentication, CreateAuditActionCode(EventTypeCodes.Logout));
            audit.Actors.Add(new AuditActorData()
            {
                NetworkAccessPointId = Dns.GetHostName(),
                NetworkAccessPointType = NetworkAccessPointType.MachineName,
                UserName = principal.Identity.Name,
                UserIsRequestor = true
            });
            AddDeviceActor(audit);
            AddSenderDeviceActor(audit);

            SendAudit(audit);
        }

        /// <summary>
        /// Audit the use of a restricted function
        /// </summary>
        public static void AuditRestrictedFunction(UnauthorizedAccessException ex, Uri url)
        {
            AuditData audit = new AuditData(DateTime.Now, ActionType.Execute, OutcomeIndicator.EpicFail, EventIdentifierType.SecurityAlert, CreateAuditActionCode(EventTypeCodes.UseOfARestrictedFunction));
            AddUserActor(audit);
            AddDeviceActor(audit);
            AddSenderDeviceActor(audit);
            audit.AuditableObjects.Add(new AuditableObject()
            {
                IDTypeCode = AuditableObjectIdType.Uri,
                LifecycleType = AuditableObjectLifecycle.Access,
                ObjectId = url.ToString(),
                Role = AuditableObjectRole.Resource,
                Type = AuditableObjectType.SystemObject
            });
            SendAudit(audit);
        }
    }
}
