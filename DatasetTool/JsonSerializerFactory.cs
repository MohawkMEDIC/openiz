/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-11-30
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Model;
using Newtonsoft.Json;
using System.Reflection;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Diagnostics;
using System.Collections;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a factory which can generate IJsonViewModelSerializers 
    /// </summary>
    public class JsonSerializerFactory : JsonReflectionTypeFormatter
    {
        private static readonly CodePrimitiveExpression s_true = new CodePrimitiveExpression(true);
        private static readonly CodePrimitiveExpression s_false = new CodePrimitiveExpression(false);
        private static readonly CodePrimitiveExpression s_null = new CodePrimitiveExpression(null);
        private static readonly CodeThisReferenceExpression s_this = new CodeThisReferenceExpression();

        private static readonly CodeVariableReferenceExpression s_serializationContext = new CodeVariableReferenceExpression("context");
        private static readonly CodeVariableReferenceExpression s_retVal = new CodeVariableReferenceExpression("_retVal");
        private static readonly CodeTypeReference s_tIdentifiedData = new CodeTypeReference(typeof(IdentifiedData));
        private static readonly CodeTypeReference s_tJsonReader = new CodeTypeReference(typeof(JsonReader));
        private static readonly CodeTypeReference s_tJsonWriter = new CodeTypeReference(typeof(JsonWriter));

        private static readonly CodeFieldReferenceExpression s_fTracer = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_tracer");
        private static readonly CodeFieldReferenceExpression s_fBinder = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_binder");
        private static readonly CodeMethodReferenceExpression s_traceError = new CodeMethodReferenceExpression(s_fTracer, "TraceError");

        /// <summary>
        /// JSON Serializer factory
        /// </summary>
        public JsonSerializerFactory() : base(typeof(Object))
        {

        }

        /// <summary>
        /// Creates a String.Format() expression
        /// </summary>
        private CodeExpression CreateStringFormatExpression(string formatString, params CodeExpression[] parms)
        {
            var retVal = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(String)), "Format"), new CodePrimitiveExpression(formatString));
            foreach (var p in parms)
                retVal.Parameters.Add(p);
            return retVal;
        }

        /// <summary>
        /// Create a instance cast expression
        /// </summary>
        private CodeStatement CreateToStringTryCatch(CodeExpression targetObject, CodeExpression sourceObject, CodeStatement failExpression)
        {
            return new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(targetObject, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(sourceObject, "ToString"))),
                },
                new CodeCatchClause[] {

                new CodeCatchClause("e", new CodeTypeReference(typeof(Exception)),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(s_traceError, this.CreateStringFormatExpression("Casting Error: {0}", new CodeVariableReferenceExpression("e")))),
                    failExpression)
                });

        }
        /// <summary>
        /// Create a instance cast expression
        /// </summary>
        private CodeStatement CreateCastTryCatch(Type toType, CodeExpression targetObject, CodeExpression sourceObject, CodeStatement failExpression)
        {
            return new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(targetObject, new CodeCastExpression(new CodeTypeReference(toType), sourceObject)),
                },
                new CodeCatchClause[] {

                new CodeCatchClause("e", new CodeTypeReference(typeof(Exception)),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(s_traceError, this.CreateStringFormatExpression("Casting Error: {0}", new CodeVariableReferenceExpression("e")))),
                    failExpression)
                });
        }

        /// <summary>
        /// Create a simaple method call expression
        /// </summary>
        private CodeExpression CreateSimpleMethodCallExpression(CodeExpression target, String methodName, params Object[] simpleParameters)
        {
            var retVal = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(target, methodName));
            foreach (var p in simpleParameters)
                retVal.Parameters.Add(new CodePrimitiveExpression(p));
            return retVal;
        }

        /// <summary>
        /// Create ConvertExpression
        /// </summary>
        private CodeExpression CreateConvertExpression(String methodName, CodeExpression parameter)
        {
            return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), methodName, parameter);
        }


        /// <summary>
        /// Create an equals statement
        /// </summary>
        private CodeMethodInvokeExpression CreateEqualsStatement(CodeExpression left, CodeExpression right)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(left, "Equals"), right);
        }


        /// <summary>
        /// Creates a call to GetType()
        /// </summary>
        private CodeExpression CreateGetTypeExpression(CodeExpression _object)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_object, "GetType"));
        }


        /// <summary>
        /// Create a code namespace for the specified assembly
        /// </summary>
        public CodeNamespace CreateCodeNamespace(String name, Assembly asm)
        {
            CodeNamespace retVal = new CodeNamespace(name);
            retVal.Imports.Add(new CodeNamespaceImport("OpenIZ.Core.Model"));

            foreach (var t in asm.GetTypes().Where(o => o.GetCustomAttribute<JsonObjectAttribute>() != null))
            {
                var ctdecl = this.CreateViewModelSerializer(t);
                if (ctdecl != null)
                    retVal.Types.Add(ctdecl);
            }
            return retVal;
        }

        /// <summary>
        /// Creates a view model serializer
        /// </summary>
        public CodeTypeDeclaration CreateViewModelSerializer(Type forType)
        {

            // Cannot process this type
            if (forType.IsGenericType && !forType.ContainsGenericParameters || forType.IsGenericTypeDefinition || forType.IsAbstract ||
                !typeof(IdentifiedData).IsAssignableFrom(forType))
                return null;

            // Generate the type definition
            CodeTypeDeclaration retVal = new CodeTypeDeclaration(String.Format("{0}ViewModelSerializer", forType.GetCustomAttribute<JsonObjectAttribute>()?.Id ?? forType.Name));
            retVal.IsClass = true;
            retVal.TypeAttributes = TypeAttributes.Public;
            retVal.BaseTypes.Add(typeof(IJsonViewModelTypeFormatter));

            // Add methods
            retVal.Members.Add(this.CreateBinderField());
            retVal.Members.Add(this.CreateTracerField(retVal));
            retVal.Members.Add(this.CreateHandlesTypeProperty(forType));
            retVal.Members.Add(this.CreateSerializeMethod(forType));
            retVal.Members.Add(this.CreateDeserializeMethod(forType));
            retVal.Members.Add(this.CreateFromSimpleValue(forType));
            retVal.Members.Add(this.CreateGetSimpleValue(forType));

            return retVal;
        }

        /// <summary>
        /// Create binder field
        /// </summary>
        private CodeTypeMember CreateBinderField()
        {
            return new CodeMemberField(typeof(ModelSerializationBinder), "m_binder")
            {
                Attributes = MemberAttributes.Private,
                InitExpression = new CodeObjectCreateExpression(typeof(ModelSerializationBinder))
            };
        }

        /// <summary>
        /// Creates the handles type property
        /// </summary>
        private CodeTypeMember CreateHandlesTypeProperty(Type forType)
        {
            var retVal = new CodeMemberProperty()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Type = new CodeTypeReference(typeof(Type)),
                HasGet = true,
                HasSet = false,
                Name = "HandlesType"
            };
            retVal.GetStatements.Add(new CodeMethodReturnStatement(new CodeTypeOfExpression(forType)));
            return retVal;
        }

        /// <summary>
        /// Create tracer property
        /// </summary>
        private CodeTypeMember CreateTracerField(CodeTypeDeclaration retVal)
        {
            return new CodeMemberField(typeof(Tracer), "m_tracer")
            {
                Attributes = MemberAttributes.Private,
                InitExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Tracer)), "GetTracer"), new CodeTypeOfExpression(new CodeTypeReference(retVal.Name)))
            };
        }

        /// <summary>
        /// Create the GetSimpleValue method
        /// </summary>
        private CodeTypeMember CreateGetSimpleValue(Type forType)
        {
            var retVal = new CodeMemberMethod()
            {
                Name = "GetSimpleValue",
                ReturnType = new CodeTypeReference(typeof(Object)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "o"));
            var _o = new CodeVariableReferenceExpression("o");

            // For type has simple value?
            if (forType.GetCustomAttribute<SimpleValueAttribute>() == null)
                retVal.Statements.Add(new CodeMethodReturnStatement(s_null));
            else
            {
                // Value is null ? return null
                retVal.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(_o, CodeBinaryOperatorType.IdentityEquality, s_null), new CodeMethodReturnStatement(s_null)));
                var _strongType = new CodeVariableReferenceExpression("_strong");
                retVal.Statements.Add(new CodeVariableDeclarationStatement(forType, "_strong", s_null));
                retVal.Statements.Add(this.CreateCastTryCatch(forType, _strongType, _o, new CodeMethodReturnStatement(s_null)));

                // Build the simple property
                retVal.Statements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(_strongType, forType.GetCustomAttribute<SimpleValueAttribute>().ValueProperty)));
            }
            return retVal;
        }

        /// <summary>
        /// Create the from simple value member
        /// </summary>
        private CodeTypeMember CreateFromSimpleValue(Type forType)
        {
            var retVal = new CodeMemberMethod()
            {
                Name = "FromSimpleValue",
                ReturnType = new CodeTypeReference(typeof(Object)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "o"));
            var _o = new CodeVariableReferenceExpression("o");

            // This has a simple value attribute?
            if (forType.GetCustomAttribute<SimpleValueAttribute>() == null)
                retVal.Statements.Add(new CodeMethodReturnStatement(s_null));
            else
            {
                // Does this match the property type?
                var propertyType = forType.GetRuntimeProperty(forType.GetCustomAttribute<SimpleValueAttribute>().ValueProperty);
                var _strongType = new CodeVariableReferenceExpression("_strong");
                retVal.Statements.Add(new CodeVariableDeclarationStatement(propertyType.PropertyType, "_strong", s_null));
                // Ensure not null
                retVal.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(_o, CodeBinaryOperatorType.IdentityEquality, s_null), new CodeMethodReturnStatement(s_null)));
                if (propertyType.PropertyType == typeof(String))
                    retVal.Statements.Add(this.CreateToStringTryCatch(_strongType, _o, new CodeMethodReturnStatement(s_null)));
                else
                    retVal.Statements.Add(this.CreateCastTryCatch(propertyType.PropertyType, _strongType, _o, new CodeMethodReturnStatement(s_null)));
                retVal.Statements.Add(new CodeVariableDeclarationStatement(forType, "_retVal", new CodeObjectCreateExpression(forType)));
                // Set the property value
                retVal.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(s_retVal, propertyType.Name), _strongType));
                retVal.Statements.Add(new CodeMethodReturnStatement(s_retVal));
            }
            return retVal;
        }

        /// <summary>
        /// Create a de-serialization method
        /// </summary>
        private CodeTypeMember CreateDeserializeMethod(Type forType)
        {
            var retVal = new CodeMemberMethod()
            {
                Name = "Deserialize",
                ReturnType = new CodeTypeReference(typeof(Object)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(JsonReader), "r"));
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Type), "asType"));
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(JsonSerializationContext), "context"));
            var _reader = new CodeVariableReferenceExpression("r");
            var _context = new CodeVariableReferenceExpression("context");
            var _jsonContext = new CodePropertyReferenceExpression(_context, "JsonContext");

            retVal.Statements.Add(new CodeVariableDeclarationStatement(forType, "_retVal", new CodeObjectCreateExpression(forType)));

            var _depth = new CodeVariableReferenceExpression("_depth");
            var readerDepth = new CodePropertyReferenceExpression(_reader, "Depth");
            var readerToken = new CodePropertyReferenceExpression(_reader, "TokenType");
            var readerValue = new CodePropertyReferenceExpression(_reader, "Value");

            // Depth
            retVal.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "_depth", readerDepth));
            retVal.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(readerToken, CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(JsonToken)), "StartObject")),
                new CodeAssignStatement(_depth, new CodeBinaryOperatorExpression(_depth, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)))));

            var jsonPropertyTokenCondition = new CodeConditionStatement(new CodeBinaryOperatorExpression(readerToken, CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(JsonToken)), "PropertyName")));
            jsonPropertyTokenCondition.FalseStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(readerToken, CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(JsonToken)), "EndObject")),
                    CodeBinaryOperatorType.BooleanAnd,
                    new CodeBinaryOperatorExpression(readerDepth, CodeBinaryOperatorType.IdentityEquality, _depth)
                ),
                new CodeStatement[] { new CodeMethodReturnStatement(s_retVal) },
                new CodeStatement[] { new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(JsonSerializationException), new CodePrimitiveExpression("JSON in invalid state"))) }
            ));
            var elementLoop = new CodeIterationStatement(new CodeSnippetStatement(), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_reader, "Read")), new CodeSnippetStatement(), jsonPropertyTokenCondition);

            // Switch the object type
            CodeConditionStatement propertyCondition = new CodeConditionStatement(this.CreateEqualsStatement(new CodePrimitiveExpression("$type"), readerValue));
            var _xsiTypeRef = new CodeVariableReferenceExpression("_type");
            propertyCondition.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(Type), "_type", new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(s_fBinder, "BindToType"), new CodePrimitiveExpression(forType.Assembly.FullName), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_reader, "ReadAsString")))));
            propertyCondition.TrueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(_xsiTypeRef, CodeBinaryOperatorType.IdentityInequality, new CodeTypeOfExpression(forType)),
                    new CodeVariableDeclarationStatement(forType, "_nretVal", new CodeCastExpression(forType, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_jsonContext, "GetFormatter"), _xsiTypeRef), "Deserialize"), _reader, _xsiTypeRef, _context))),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("_nretVal"), "CopyObjectData"), s_retVal)),
                    new CodeMethodReturnStatement(new CodeVariableReferenceExpression("_nretVal"))
            ));
            propertyCondition.FalseStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_reader, "Skip"))));

            // Loop
            foreach (var pi in forType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {

                var jsonName = this.GetPropertyName(pi, true);
                if (jsonName == null || jsonName.StartsWith("$") || !pi.CanWrite) continue;
                propertyCondition = new CodeConditionStatement(this.CreateEqualsStatement(new CodePrimitiveExpression(jsonName), readerValue), new CodeStatement[] { }, new CodeStatement[] { propertyCondition });
                propertyCondition.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_reader, "Read")));
                propertyCondition.TrueStatements.Add(new CodeVariableDeclarationStatement(pi.PropertyType, "_instance", new CodeCastExpression(pi.PropertyType, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_jsonContext, "ReadElementUtil"), _reader, new CodeTypeOfExpression(pi.PropertyType), new CodeObjectCreateExpression(typeof(JsonSerializationContext), new CodePrimitiveExpression(jsonName), _jsonContext, s_retVal, _context)))));
                var _instance = new CodeVariableReferenceExpression("_instance");
                propertyCondition.TrueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(_instance, CodeBinaryOperatorType.IdentityInequality, s_null), new CodeAssignStatement(new CodePropertyReferenceExpression(s_retVal, pi.Name), _instance)));
            }

            jsonPropertyTokenCondition.TrueStatements.Add(propertyCondition);
            retVal.Statements.Add(elementLoop);
            retVal.Statements.Add(new CodeMethodReturnStatement(s_retVal));
            return retVal;

        }

        /// <summary>
        /// Create serialization method
        /// </summary>
        private CodeTypeMember CreateSerializeMethod(Type forType)
        {
            var retVal = new CodeMemberMethod()
            {
                Name = "Serialize",
                ReturnType = new CodeTypeReference(typeof(void)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(JsonWriter), "w"));
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IdentifiedData), "o"));
            retVal.Parameters.Add(new CodeParameterDeclarationExpression(typeof(JsonSerializationContext), "context"));
            var _object = new CodeVariableReferenceExpression("o");
            var _writer = new CodeVariableReferenceExpression("w");
            var _context = new CodeVariableReferenceExpression("context");

            // Verify the object is not null
            retVal.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(_object, CodeBinaryOperatorType.IdentityEquality, s_null), new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentNullException), new CodePrimitiveExpression("o")))));

            // Cast
            var _strongType = new CodeVariableReferenceExpression("_strong");
            var _strongKeyReference = new CodePropertyReferenceExpression(_strongType, "Key");
            var _loaded = new CodeVariableReferenceExpression("_loaded");
            var _jsonContext = new CodePropertyReferenceExpression(_context, "JsonContext");
            retVal.Statements.Add(new CodeVariableDeclarationStatement(forType, "_strong", s_null));
            retVal.Statements.Add(new CodeVariableDeclarationStatement(typeof(JsonSerializationContext), "_jsonContext", s_null));
            retVal.Statements.Add(new CodeVariableDeclarationStatement(typeof(bool), "_loaded", s_false));
            retVal.Statements.Add(this.CreateCastTryCatch(forType, _strongType, _object, new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentException), this.CreateStringFormatExpression("Invalid type {0} provided, expected {1}", this.CreateGetTypeExpression(_object), new CodeTypeOfExpression(forType))))));

            // Iterate through the object constructing the properties
            foreach (var pi in forType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var jsonName = this.GetPropertyName(pi);
                if (jsonName == null || jsonName.StartsWith("$") || !pi.CanRead) continue;

                // Create an if statement that represents whether we should serialize
                var shouldSerializeCondition = new CodeConditionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_context, "ShouldSerialize"), new CodePrimitiveExpression(jsonName)));
                retVal.Statements.Add(shouldSerializeCondition);

                // Check if the property is null
                var _propertyReference = new CodePropertyReferenceExpression(_strongType, pi.Name);
                CodeBinaryOperatorExpression isNullCondition = new CodeBinaryOperatorExpression(_propertyReference, CodeBinaryOperatorType.IdentityEquality, s_null);
                if (typeof(IList).IsAssignableFrom(pi.PropertyType) && !pi.PropertyType.IsArray)
                    isNullCondition = new CodeBinaryOperatorExpression(isNullCondition, CodeBinaryOperatorType.BooleanOr, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(_propertyReference, "Count"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)));
                var nullPropertyValueCondition = new CodeConditionStatement(isNullCondition);
                shouldSerializeCondition.TrueStatements.Add(nullPropertyValueCondition);

                // Method expression to call WritePropertyUtil
                var writePropertyCall = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_jsonContext, "WritePropertyUtil"), _writer, new CodePrimitiveExpression(jsonName), _propertyReference, _context);

                // Should we delay load?
                if (typeof(IIdentifiedEntity).IsAssignableFrom(pi.PropertyType.StripGeneric()))
                {
                    var shouldForceLoadCondition = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(_strongKeyReference, "HasValue"), CodeBinaryOperatorType.BooleanAnd, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_context, "ShouldForceLoad"), new CodePrimitiveExpression(jsonName), new CodePropertyReferenceExpression(_strongKeyReference, "Value")));
                    //if(typeof(IVersionedEntity).IsAssignableFrom(forType))
                    //    shouldForceLoadCondition = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(_strongType, "VersionKey"), "HasValue"), CodeBinaryOperatorType.BooleanAnd, shouldForceLoadCondition);
                    var shouldForceLoad = new CodeConditionStatement(shouldForceLoadCondition);
                    // Check persistence
                    nullPropertyValueCondition.TrueStatements.Add(shouldForceLoad);
                    var _delay = new CodeVariableReferenceExpression("_delay");
                    shouldForceLoad.TrueStatements.Add(new CodeVariableDeclarationStatement(pi.PropertyType, "_delay", s_null));
                    CodeExpression wasLoadedExpression = null;
                    var _strongKeyReferenceValue = new CodePropertyReferenceExpression(_strongKeyReference, "Value");
                    if (typeof(IList).IsAssignableFrom(pi.PropertyType) && !pi.PropertyType.IsArray)
                    {
                        if (typeof(ISimpleAssociation).IsAssignableFrom(pi.PropertyType.StripGeneric()))
                        {
                            shouldForceLoad.TrueStatements.Add(new CodeAssignStatement(_delay, new CodeObjectCreateExpression(pi.PropertyType, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_jsonContext, "LoadCollection", new CodeTypeReference(pi.PropertyType.StripGeneric())), _strongKeyReferenceValue))));
                            wasLoadedExpression = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(_delay, "Count"), CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0));
                        }
                    }
                    else
                    {
                        var keyPropertyRef = pi.GetCustomAttribute<SerializationReferenceAttribute>();
                        if (keyPropertyRef != null)
                        {
                            shouldForceLoad.TrueStatements.Add(new CodeAssignStatement(_delay, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_jsonContext, "LoadRelated", new CodeTypeReference(pi.PropertyType)), new CodePropertyReferenceExpression(_strongType, keyPropertyRef.RedirectProperty))));
                            wasLoadedExpression = new CodeBinaryOperatorExpression(_delay, CodeBinaryOperatorType.IdentityInequality, s_null);
                        }
                    }

                    if (wasLoadedExpression != null)
                    {
                        
                        shouldForceLoad.TrueStatements.Add(new CodeConditionStatement(wasLoadedExpression,
                            new CodeStatement[] { new CodeAssignStatement(_propertyReference, _delay), new CodeAssignStatement(_loaded, s_true), new CodeExpressionStatement(writePropertyCall) },
                            new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(_context, "RegisterMissTarget"), new CodePrimitiveExpression(jsonName), new CodePropertyReferenceExpression(_strongKeyReference, "Value"))) }
                        ));
                    }
                }
                nullPropertyValueCondition.FalseStatements.Add(writePropertyCall);

            }

            // Do we need to write loaded properties
            var _loadStateReference = new CodePropertyReferenceExpression(_strongType, nameof(IdentifiedData.LoadState));
            var _newLoadStateReference = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(LoadState)), nameof(LoadState.New));
            var shouldUpdateCacheExpression = new CodeBinaryOperatorExpression(_loaded, CodeBinaryOperatorType.BooleanAnd, 
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(_newLoadStateReference, CodeBinaryOperatorType.IdentityInequality, _loadStateReference), 
                    CodeBinaryOperatorType.BooleanAnd, 
                    new CodePropertyReferenceExpression(_strongKeyReference, "HasValue")
                )
            );
            if (typeof(IVersionedEntity).IsAssignableFrom(forType))
                shouldUpdateCacheExpression = new CodeBinaryOperatorExpression(shouldUpdateCacheExpression, CodeBinaryOperatorType.BooleanAnd, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(_strongType, "VersionKey"), "HasValue"));
            retVal.Statements.Add(new CodeConditionStatement(shouldUpdateCacheExpression,
                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeCastExpression(typeof(IDataCachingService), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(ApplicationServiceContext)), "Current"), "GetService"), new CodeTypeOfExpression(typeof(IDataCachingService)))), "Add"), _strongType))));
            return retVal;
        }

    }
}
