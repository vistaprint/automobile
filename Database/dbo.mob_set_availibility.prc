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

dbo.mob_set_avalibility.prc
*/
IF  EXISTS (SELECT 1 FROM sysobjects with(nolock) WHERE id = OBJECT_ID(N'[dbo].[mob_set_availibility]'))
DROP PROCEDURE [dbo].[mob_set_availibility]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[mob_set_availibility]
(
	@DEVICE_ID varchar(128),
	@AVALIBILITY bit
)
AS
BEGIN
SET NOCOUNT ON

UPDATE dbo.mob_device_register SET availible = @AVALIBILITY WHERE device_id = @DEVICE_ID  

END


GO
IF EXISTS (SELECT 1 FROM sysobjects with(nolock) WHERE name = 'adm_reset_permissions')
EXEC adm_reset_permissions 'mob_set_availibility'
