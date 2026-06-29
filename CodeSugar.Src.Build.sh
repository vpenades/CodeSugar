# this script can be run directly or from the github actions.

# stop on first error
set -e

# set input or default

DEFAULTPACKAGEVERSION="1.0.0-Test-DATE-TIME"
PACKAGEVERSION=${1:-$DEFAULTPACKAGEVERSION}

# replace date
DATE_SHORT=$(date +'%Y%m%d')
PACKAGEVERSION="${PACKAGEVERSION/DATE/$DATE_SHORT}"

# replace time
TIME_SHORT=$(date +'%H%M%S')
PACKAGEVERSION="${PACKAGEVERSION/TIME/$TIME_SHORT}"

# extract suffix
PACKAGESUFFIX="${PACKAGEVERSION#*-}"

# report semver
echo "package version: $PACKAGEVERSION";
echo "package suffix: $PACKAGESUFFIX";

# restore

dotnet clean
dotnet tool restore
dotnet restore

# generate decoupled solutions for src and tests to avoid SourceGenerator build issues

dotnet slngen src/*/*.csproj -o src/.src.sln --launch false

dotnet build src/.src.sln

dotnet test CodeSugar.slnx

dotnet PackAsSourcesNuget src/.src.sln -o . --package-version $PACKAGEVERSION --append-sources-suffix

# read -p "Press [ENTER] to continue..."