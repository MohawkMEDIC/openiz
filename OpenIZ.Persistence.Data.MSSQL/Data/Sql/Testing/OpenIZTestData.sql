DELETE FROM SecurityUserRole WHERE UserId IN (SELECT UserId FROM SecurityUser WHERE UserName IN ('Bob','Lucy', 'SyncUser', 'Administrator'));
DELETE FROM SecurityUser WHERE UserName IN ('Bob','Lucy', 'SyncUser', 'Administrator');
DELETE FROM SecurityApplicationPolicy WHERE ApplicationId IN (SELECT ApplicationId FROM SecurityApplication WHERE ApplicationPublicId = 'fiddler');
DELETE FROM SecurityApplication WHERE ApplicationPublicId = 'fiddler';


INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Administrator', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'mailto:administrator@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityUserRole (UserId, RoleId)
	SELECT UserId, RoleId FROM SecurityUser, SecurityRole 
	WHERE SecurityUser.UserName = 'Administrator' AND SecurityRole.Name IN ('ADMINISTRATORS');

INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Bob', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'mailto:bob@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityUserRole (UserId, RoleId)
	SELECT UserId, RoleId FROM SecurityUser, SecurityRole 
	WHERE SecurityUser.UserName = 'Bob' AND SecurityRole.Name IN ('USERS');

INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Lucy', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'mailto:lucy@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

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