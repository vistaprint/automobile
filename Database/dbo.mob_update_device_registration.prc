/*
Copyright 2013 Vistaprint

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

dbo.mob_update_device_registration.prc
*/

IF  EXISTS (SELECT 1 FROM sysobjects with(nolock) WHERE id = OBJECT_ID(N'[dbo].[mob_update_device_registration]'))
DROP PROCEDURE [dbo].[mob_update_device_registration]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[mob_update_device_registration]
(
	@DEVICE_ID varchar(128),
	@OS varchar(64),
	@OS_VERSION varchar(32),
	@IP varchar(16)
)
AS
BEGIN
BEGIN TRANSACTION
SET NOCOUNT ON
	if not exists (SELECT 1 FROM [dbo].[mob_device_register] WITH (updlock, rowlock, holdlock) WHERE device_id = @DEVICE_ID )
	BEGIN
		INSERT INTO [dbo].[mob_device_register](device_id, mobile_os, os_version, ip, availible) VALUES (@DEVICE_ID, @OS, @OS_VERSION, @IP, 1)
	END
	else
	BEGIN
		UPDATE [dbo].[mob_device_register] SET ip = @IP, availible = 1 WHERE device_id = @DEVICE_ID
	END
COMMIT TRANSACTION
END


GO
IF EXISTS (SELECT 1 FROM sysobjects with(nolock) WHERE name = 'adm_reset_permissions')
EXEC adm_reset_permissions 'mob_update_device_registration'
