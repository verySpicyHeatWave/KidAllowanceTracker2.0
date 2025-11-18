RUN_SETTINGS=Tests/AllowanceApp.Tests/Settings/Test.runsettings
REPORT_FILE=coverage.cobertura.xml
RESULTS_PATH=Tests/AllowanceApp.Tests/TestResults
COV_PATH=Tests/AllowanceApp.Tests/Coverage
DB_FILE=~/.local/share/accounts.db
MY_CSS=./Source/AllowanceApp.Blazor/wwwroot/app.css
OUT_CSS=./Source/AllowanceApp.Blazor/wwwroot/dist.css

.PHONY: build test coverage run_api run_blazor run clean wiped spotless dbclean refresh-css watch-css docker_api ci

ci:
	@dotnet format --verify-no-changes --severity warn

build:
	@dotnet build

test: build
	@$(MAKE) quicktest

coverage: test
	@echo "Finding latest coverage file..."
	@COV_FILE=$$(ls -t $(RESULTS_PATH)/**/$(REPORT_FILE) | head -n1); \
	echo "Generating coverage report for file $$COV_FILE"; \
	reportgenerator -reports:"$$COV_FILE" \
    	-targetdir:"$(COV_PATH)" \
    	-reporttypes:Html

quicktest:
	@dotnet test --settings $(RUN_SETTINGS) --no-build --collect:"XPlat Code Coverage"

quickcov: quicktest
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
	@echo "Starting the front end..."
	@dotnet watch --project Source/AllowanceApp.Blazor

run: run_api

refresh-css:
	@npx @tailwindcss/cli -i $(MY_CSS) -o $(OUT_CSS)

watch-css:
	@npx @tailwindcss/cli -i $(MY_CSS) -o $(OUT_CSS) --watch

docker-api: 
	@docker run -d \
		--name allowance-api \
		--network allowance-net \
		-p 8080:8080 \
		-e DOTNET_ENVIRONMENT=Release \
		-v ~/.local/share:/app/database \
		allowance-api

docker-blazor: 
	docker run -d \
		--name allowance-blazor \
		--network allowance-net \
		-p 5000:5000 \
		-e DOTNET_ENVIRONMENT=Release \
		-e DOTNET_URLS=http://0.0.0.0:5000 \
		-e ParentAuth__PasswordHash="AQAAAAIAAYagAAAAEKVERRNet6qnUvwWLaWVervZXOlo4gTeyC0fXRTH+uxRAJAG0Hvi8Kl6WMhf/f1FRg==" \
		allowance-blazor

docker-api: 
	docker run -d \
		--name allowance-api \
		--network allowance-net \
		-p 8080:8080 \
		-e DOTNET_ENVIRONMENT=Release \
		-v /srv/allowance-app:/app/database \
		allowance-api