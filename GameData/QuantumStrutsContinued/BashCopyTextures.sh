#!/bin/bash

#
# This script needs to be run on Linux and OSX to copy
# the missing texture files.  Double-clicking on it
# should be sufficient

clear

echo -e "\n\nThis batch file will copy the missing textures from the inaccessable"
echo "Squad/zDeprecated directory "
echo -e "\n\n"
echo "Press return to continue"
read yn

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
[ ! -d "$DIR/Squad" ] && mkdir $DIR/Squad
cd $DIR/Squad

mkdir linearRCS_v1
cp ../../Squad/zDeprecated/Parts/Utility/linearRCS_v1/model.mu linearRCS_v1
cp ../../Squad/zDeprecated/Parts/Utility/linearRCS_v1/rcs.dds linearRCS_v1



echo -e "/n/nThe files have been copied/n"
echo "Press return to continue"
read yn
