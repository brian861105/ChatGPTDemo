#!/bin/bash
# This script is used to run the test suite for the backend.

# Usage: ./test.sh
dotnet test  /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura 

reportgenerator \
-reports:"AuthServer/AuthServer.Tests/coverage.cobertura.xml" \
-targetdir:"coveragereport" \
-reporttypes:Html