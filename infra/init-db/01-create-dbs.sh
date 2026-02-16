#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE outclass_tenant;
    CREATE DATABASE outclass_metadata;
    CREATE DATABASE outclass_document;
    CREATE DATABASE outclass_workflow;
    CREATE DATABASE outclass_automation;
    CREATE DATABASE outclass_filestorage;
EOSQL
