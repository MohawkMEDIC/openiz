DELETE FROM SecurityUserRole WHERE UserId IN (SELECT UserId FROM SecurityUser WHERE UserName IN ('Bob','Lucy', 'SyncUser', 'Administrator'));
DELETE FROM SecurityUser WHERE UserName IN ('Bob','Lucy', 'SyncUser', 'Administrator');
DELETE FROM SecurityApplicationPolicy WHERE ApplicationId IN (SELECT ApplicationId FROM SecurityApplication WHERE ApplicationPublicId = 'fiddler');
DELETE FROM SecurityApplication WHERE ApplicationPublicId = 'fiddler';

INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Administrator', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'administrator@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityUserRole (UserId, RoleId)
	SELECT UserId, RoleId FROM SecurityUser, SecurityRole 
	WHERE SecurityUser.UserName = 'Administrator' AND SecurityRole.Name IN ('ADMINISTRATORS');

INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Bob', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'bob@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityUserRole (UserId, RoleId)
	SELECT UserId, RoleId FROM SecurityUser, SecurityRole 
	WHERE SecurityUser.UserName = 'Bob' AND SecurityRole.Name IN ('USERS');

INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Lucy', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'lucy@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityUserRole (UserId, RoleId)
	SELECT UserId, RoleId FROM SecurityUser, SecurityRole 
	WHERE SecurityUser.UserName = 'Lucy' AND SecurityRole.Name IN ('CLINICAL_STAFF');

INSERT INTO SecurityApplication (ApplicationPublicId, ApplicationSecret, CreatedBy)
	VALUES ('fiddler','0180cad1928b9b9887a60a123920a793e7aa7cd339577876f0c233fa2b9fb7d6', 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityApplicationPolicy(ApplicationId, PolicyId, PolicyAction)
	SELECT ApplicationId, PolicyId, 2 FROM
		SecurityApplication, Policy
	WHERE
		SecurityApplication.ApplicationPublicId = 'fiddler';

INSERT INTO SecurityApplication (ApplicationPublicId, ApplicationSecret, CreatedBy)
	VALUES ('org.openiz.openiz_mobile', ('ec1e5ef79b95cc1e8a5dec7492b9eb7e2b413ad7a45c5637d16c11bb68fcd53c'), 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityApplicationPolicy(ApplicationId, PolicyId, PolicyAction)
	SELECT ApplicationId, PolicyId, 2 FROM
		SecurityApplication, Policy
	WHERE
		SecurityApplication.ApplicationPublicId = 'org.openiz.openiz_mobile';

INSERT INTO SecurityDevice (DeviceId, DeviceSecret, DevicePublicId, CreatedBy)		
	VALUES ('F90F1488-F3CC-4357-9462-7CE3AB12B148', 'device_secret', 'device_public_id', 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO AssigningAuthority (AssigningAuthorityId, Name, Oid, HL7CX4, Url, AssigningDeviceId, [Description], CreatedBy)
		VALUES (NEWID(), 'Test', '1.3.6.1.4.1.33349.3.1.5.9.2.10000', 'Test', 'http://marc-hi.ca', 'F90F1488-F3CC-4357-9462-7CE3AB12B148', 'Testing Device', 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityApplication (ApplicationId, ApplicationPublicId, ApplicationSecret, CreationTime, CreatedBy) 
		VALUES ('4C5A581C-A6EE-4267-9231-B0D3D50CC08A', 'org.openiz.minims', 'cba830db9a6f5a4b638ff95ef70e98aa82d414ac35b351389024ecb6be40ebf0', GETUTCDATE(), 'FADCA076-3690-4A6E-AF9E-F1CD68E8C7E8');

INSERT INTO SecurityApplicationPolicy
		VALUES ('638E74A4-42E7-45E0-B693-96048DCA7692', '4C5A581C-A6EE-4267-9231-B0D3D50CC08A', 'D15B96AB-646C-4C00-9A58-EA09EEE67D7C', 2);

INSERT INTO SecurityApplication (ApplicationPublicId, ApplicationSecret, CreatedBy)
	VALUES ('org.openiz.minims', ('cba830db9a6f5a4b638ff95ef70e98aa82d414ac35b351389024ecb6be40ebf0'), 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityApplicationPolicy(ApplicationId, PolicyId, PolicyAction)
	SELECT ApplicationId, PolicyId, 2 FROM
		SecurityApplication, Policy
	WHERE
		SecurityApplication.ApplicationPublicId = 'org.openiz.minims';