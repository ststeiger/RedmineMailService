
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__T_EML_MailDelivery]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.__T_EML_MailDelivery
(
	 EML_UID uniqueidentifier NULL
	,EML_Module national character varying(500) NULL
	,EML_From character varying(255) NULL
	,EML_To character varying(255) NULL
	,EML_Sent datetime NULL
);
END
GO
