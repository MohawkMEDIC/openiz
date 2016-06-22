DELETE FROM SecurityUserRole WHERE UserId IN (SELECT UserId FROM SecurityUser WHERE UserName = 'Administrator');
DELETE FROM SecurityUser WHERE UserName = 'Administrator';
DELETE FROM SecurityApplicationPolicy WHERE ApplicationId IN (SELECT ApplicationId FROM SecurityApplication WHERE ApplicationPublicId = 'fiddler');
DELETE FROM SecurityApplication WHERE ApplicationPublicId = 'fiddler';


INSERT INTO SecurityUser (UserName, SecurityStamp, UserPassword, Email, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, CreatedBy)
	VALUES ('Administrator', NEWID(), '59ff5973691ff75f8baa45f1e38fae24875f77ef00987ed22b02df075fb144f9', 'mailto:administrator@marc-hi.ca', 'tel:+19055751212;ext=4085', 1, 1, 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityUserRole (UserId, RoleId)
	SELECT UserId, RoleId FROM SecurityUser, SecurityRole 
	WHERE SecurityUser.UserName = 'Administrator' AND SecurityRole.Name IN ('ADMINISTRATORS','SYNCHRONIZERS');

INSERT INTO SecurityApplication (ApplicationPublicId, ApplicationSecret, CreatedBy)
	VALUES ('fiddler','0180cad1928b9b9887a60a123920a793e7aa7cd339577876f0c233fa2b9fb7d6', 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8');

INSERT INTO SecurityApplicationPolicy(ApplicationId, PolicyId, PolicyAction)
	SELECT ApplicationId, PolicyId, 2 FROM
		SecurityApplication, Policy
	WHERE
		SecurityApplication.ApplicationPublicId = 'fiddler';