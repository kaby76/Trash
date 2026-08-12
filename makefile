build:
	dotnet build-server shutdown
	rm -rf src/*/bin src/*/obj
	dotnet restore --ignore-failed-sources
	dotnet build -c Release
	bash scripts/pack-trash.sh

install:
	bash scripts/setup.sh
	bash scripts/install-local.sh

pack-trash:
	bash scripts/pack-trash.sh

clean:
	dotnet build-server shutdown
	-bash scripts/uninstall.sh 2> /dev/null
	-bash scripts/unsetup.sh 2> /dev/null
	-rm -rf nuget.config 2> /dev/null
	-rm -rf src/trash/staging 2> /dev/null
	-find . -name obj -type d -exec rm -rf '{}' ';' 2> /dev/null
	-find . -name bin -type d -exec rm -rf '{}' ';' 2> /dev/null
	-cd tests; find . -name Generated -type d -exec rm -rf '{}' ';' 2> /dev/null
	-dotnet nuget locals all --clear

publish:
	bash scripts/publish.sh
