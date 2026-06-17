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

# build

dotnet tool restore
dotnet restore

dotnet test -c Release CodeSugar.slnx

dotnet pack -c Release CodeSugar.slnx -o . --version $PACKAGEVERSION

# read -p "Press [ENTER] to continue..."