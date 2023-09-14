build:
	rm -rf src/*/bin src/*/obj
	dotnet restore --ignore-failed-sources
	dotnet build -c Release

install:
	-bash _scripts/setup.sh
	-bash _scripts/install.sh

clean:
	-bash _scripts/uninstall.sh 2> /dev/null
	-bash _scripts/unsetup.sh 2> /dev/null
	-rm -rf nuget.config 2> /dev/null
	-find . -name obj -type d -exec rm -rf '{}' ';' 2> /dev/null
	-find . -name bin -type d -exec rm -rf '{}' ';' 2> /dev/null
	-cd _tests; find . -name Generated -type d -exec rm -rf '{}' ';' 2> /dev/null

publish:
	bash _scripts/publish.sh
