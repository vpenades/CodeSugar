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

# report semver
echo "package version: $PACKAGEVERSION";

# build

# stop on first error
set -e

dotnet tool restore
dotnet restore

dotnet test -c Release CodeSugar.slnx
dotnet PackAsSourcesNuget CodeSugar.slnx -o . --package-version $PACKAGEVERSION --append-sources-suffix