﻿CREATE USER [EAS1920_RO_User]
    WITH PASSWORD = N'$(ROUserPassword)';
GO
GRANT CONNECT TO [EAS1920_RO_User]
GO

