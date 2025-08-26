RUN_SETTINGS=Tests/AllowanceApp.Tests/Settings/Test.runsettings
REPORT_FILE=coverage.cobertura.xml
RESULTS_PATH=Tests/AllowanceApp.Tests/TestResults
COV_PATH=Tests/AllowanceApp.Tests/Coverage
DB_FILE=~/.local/share/accounts.db

.PHONY: test coverage run_api run_blazor run clean wiped spotless dbclean

test:
	@dotnet test --settings $(RUN_SETTINGS) --collect:"XPlat Code Coverage"

coverage: test
	@echo "Finding latest coverage file..."
	@COV_FILE=$$(ls -t $(RESULTS_PATH)/**/$(REPORT_FILE) | head -n1); \
	echo "Generating coverage report for file $$COV_FILE"; \
	reportgenerator -reports:"$$COV_FILE" \
    	-targetdir:"$(COV_PATH)" \
    	-reporttypes:Html

clean:
	@echo "Letting dotnet clean itself..."
	@dotnet clean
	@echo "Deleting test result coverage XML files..."
	@rm -rfv $(RESULTS_PATH)/*

wiped: clean
	@echo "Deleting the test coverage..."
	@rm -rfv $(COV_PATH)/*

spotless: wiped
	@echo "Fully removing obj and bin directories..."
	@rm -rfv **/bin **/obj

dbclean:
	@echo "Deleting the existing database..."
	@rm -rv $(DB_FILE)
	@echo "Rebuilding database schema..."
	@dotnet ef database update --project Source/AllowanceApp.Data

run_api:
	@echo "Starting the API..."
	@dotnet run --project Source/AllowanceApp.Api

run_blazor:
	@echo "Starting the API..."
	@dotnet run --project Source/AllowanceApp.Blazor

run: run_api