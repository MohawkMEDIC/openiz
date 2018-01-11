-- 
-- Copyright 2015-2018 Mohawk College of Applied Arts and Technology
-- 
-- 
-- Licensed under the Apache License, Version 2.0 (the "License"); you 
-- may not use this file except in compliance with the License. You may 
-- obtain a copy of the License at 
-- 
-- http://www.apache.org/licenses/LICENSE-2.0 
-- 
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
-- WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
-- License for the specific language governing permissions and limitations under 
-- the License.
-- 
-- User: khannan
-- Date: 2017-1-15
-- 

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- CORRECTS THE BYTEA ENCODING PROBLEM ON POSTGRESQL 9.X
SET bytea_output = ESCAPE;

-- @TABLE
-- REPORT FORMAT TABLE
--
-- PURPOSE: A REPORT FORMAT REPRESENTS A FORMAT IN WHICH A REPORTING ENGINE WILL PRODUCE A REPORT
--
CREATE TABLE report_format
(
	id uuid PRIMARY KEY,
	creation_time timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
	name varchar(64) NOT NULL
);

-- @TABLE
-- REPORT DEFINITION TABLE
--
-- PURPOSE: A REPORT DEFINITION REPRESENTS METADATA ABOUT A REPORT TO USE IN CONJUNCTION WITH A REPORTING ENGINE
--
CREATE TABLE report_definition
(
	id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
	author varchar(256) NOT NULL,
	correlation_id text NOT NULL,
	creation_time timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
	description varchar(1024) NULL,
	name varchar(256) NOT NULL
);

-- @TABLE
-- PARAMETER TYPE TABLE
-- 
-- PURPOSE: A PARAMETER TYPE REPRESENTS A TYPE FOR A GIVEN REPORT PARAMETER
--
CREATE TABLE parameter_type
(
	id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
	creation_time timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
	type varchar NOT NULL,
	values_provider varchar
);

-- @TABLE
-- REPORT PARAMETER TABLE
-- 
-- PURPOSE: A REPORT PARAMETER REPRESENTS A PARAMETER FOR A REPORT TO BE EXECUTED AGAINST A REPORTING ENGINE
--
CREATE TABLE report_parameter
(
	id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
	creation_time timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
	correlation_id text NOT NULL,
	description varchar(1024) NULL,
	is_nullable boolean NOT NULL,
	name varchar(256) NOT NULL,
	position int NOT NULL,
	parameter_type_id uuid NOT NULL REFERENCES parameter_type ON UPDATE NO ACTION ON DELETE NO ACTION,
	report_id uuid NOT NULL REFERENCES report_definition ON UPDATE NO ACTION ON DELETE NO ACTION,
	value bytea NULL
);

-- @TABLE
-- REPORT DEFINITION REPORT FORMAT ASSOCIATION TABLE
-- 
-- PURPOSE: A REPORT DEFINITION REPORT PARAMETER ASSOCATION REPRESENTS A REPORT DEFINTION SUPPORTING THE FOLLOWING REPORT FORMATS AND VICE VERSA
--
CREATE TABLE report_definition_report_format_association
(
	report_definition_id uuid REFERENCES report_definition ON UPDATE NO ACTION ON DELETE NO ACTION,
	report_format_id uuid REFERENCES report_format ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT report_definition_report_format_association_key PRIMARY KEY (report_definition_id, report_format_id)
);
