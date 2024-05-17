# this script can be run directly or from the github actions.

# set input or default

DEFAULTPACKAGEVERSION="1.0.0-Test-DATE-TIME"
PACKAGEVERSION=${1:-$DEFAULTPACKAGEVERSION}

# replace date
DATE_SHORT=$(date +'%Y%m%d')
PACKAGEVERSION="${PACKAGEVERSION/DATE/$DATE_SHORT}"

# replace time
TIME_SHORT=$(date +'%H%M%S')
PACKAGEVERSION="${PACKAGEVERSION/TIME/$TIME_SHORT}"

# report semver
echo "package version: $PACKAGEVERSION";

# build

dotnet tool restore
dornet restore

dotnet test -c Release CodeSugar.sln
dotnet PackAsSourcesNuget CodeSugar.sln -o . --package-version $PACKAGEVERSION