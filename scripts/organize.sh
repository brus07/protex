#!/bin/bash

# script will create user folder and copy temporary source to that folder

function print_help()
{
    cat <<EOF
Usage `basename $0`:

 -h | --help         prints this help and exits
 -s | --source       filename of user sources in tmp/ folder
 -n | --foldername   name of folder in solutions/ directory
 -o | --overwrite    remove previous folder if there's a folder with this name
EOF
}

# custom errors
E_FOLDER_EXISTS=2
E_INTERNAL_ERROR=1
E_NO_SOURCES=3

# parse input data using GNU getopt library
options=$(getopt -o hs:f:o --longoptions source:,foldername:,overwrite,help -- "$@")

# now parse parsed options
while true ; do
    case "$1" in
	-h|--help) print_help; exit 0 ;;
	-s|--source) source=$2; shift 2 ;;
	-n|--foldername) foldername=$2; shift 2 ;;
	-o|--overwrite) overwrite=1; shift ;;
	--) shift; break ;;
	# redirect error to stderr and exit with error
	*) echo "Internal error!" >&2; exit $E_INTERNAL_ERROR;
    esac
done

# if overwrite option wasn't set, than
# set it's default value to 0
overwrite=${overwrite-"0"}

# if folder already exists
if [ -e $foldername ]; then
    # check if user specified to overwrite it
    if [ "$overwrite" -eq "1" ]; then
	rm -r $foldername
    else
	# otherwise just exit with error message
	echo "Folder already exists!" > &2
	exit $E_FOLDER_EXISTS
    fi
fi

if [ ! -e $source ]; then
    echo "No specified sources found" > &2
    exit $E_NO_SOURCES
fi

# -----------------------------
# -------- main logic ---------
# -----------------------------

new_folder="~/solutions/$foldername/"

# create folder in solutions directory
mkdir $new_folder
# move source file there
mv "~/tmp/$source" $new_folder

echo

# exit with success
exit 0

