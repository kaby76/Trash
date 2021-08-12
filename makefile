build:
	rm -rf */bin */obj
	dotnet restore --ignore-failed-sources
	dotnet build

install:
	-bash _scripts/setup.sh
	-bash _scripts/uninstall.sh
	-bash _scripts/install.sh

clean:
	-rm -rf */obj */bin
	-bash _scripts/uninstall.sh
	-bash _scripts/unsetup.sh

