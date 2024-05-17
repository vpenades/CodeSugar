# this script can be run directly or from the github actions.

# set input or default

DEFAULTVERSIONSUFFIX="Test-DATE-TIME"
VERSIONSUFFIX=${1:-$DEFAULTVERSIONSUFFIX}

# replace date
DATE_SHORT=$(date +'%Y%m%d')
VERSIONSUFFIX="${VERSIONSUFFIX/DATE/$DATE_SHORT}"

# replace time
TIME_SHORT=$(date +'%H%M%S')
VERSIONSUFFIX="${VERSIONSUFFIX/TIME/$TIME_SHORT}"

# report semver
echo "version suffix: $VERSIONSUFFIX";

# build

dotnet tool restore
dornet restore

dotnet test -c Release CodeSugar.sln
dotnet PackAsSourcesNuget CodeSugar.sln -o . --version-suffix $VERSIONSUFFIX