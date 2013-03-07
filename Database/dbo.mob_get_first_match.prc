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

dbo.mob_get_first_match.prc
*/
IF  EXISTS (SELECT 1 FROM sysobjects with(nolock) WHERE id = OBJECT_ID(N'[dbo].[mob_get_first_match]'))
DROP PROCEDURE [dbo].[mob_get_first_match]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[mob_get_first_match]
(
	@DEVICE_ID varchar(32) = NULL,
	@OS varchar(64) = NULL,
	@OS_VERSION varchar(32) = NULL,
	@IP varchar(16) = NULL,
	@AVAILIBLE bit = 1
)
AS
BEGIN
SET NOCOUNT ON

DECLARE @sql        nvarchar(MAX),
        @paramlist  nvarchar(4000)
        
SELECT @sql =  'SELECT TOP 1 d.device_id, d.mobile_os, d.os_version, d.ip 
				FROM dbo.mob_device_register d
				WHERE 1 = 1'

if @DEVICE_ID IS NOT NULL
	SELECT @sql = @sql + ' AND d.device_id = @DEVICE_ID'

if @OS IS NOT NULL
	SELECT @sql = @sql + ' AND d.os = @OS'
	
if @OS_VERSION IS NOT NULL
	SELECT @sql = @sql + ' AND d.os_version = @OS_VERSION'
	
if @IP IS NOT NULL
	SELECT @sql = @sql + ' AND d.ip = @IP'
	
if @AVAILIBLE = 1
	SELECT @sql = @sql + ' AND d.availible = 1'

SELECT @paramlist ='@DEVICE_ID varchar(32),
					@OS varchar(64),
					@OS_VERSION varchar(32),
					@IP varchar(16)'

EXEC sp_executesql @sql, @paramlist, @DEVICE_ID, @OS, @OS_VERSION, @IP

END


GO
IF EXISTS (SELECT 1 FROM sysobjects with(nolock) WHERE name = 'adm_reset_permissions')
EXEC adm_reset_permissions 'mob_get_first_match'
