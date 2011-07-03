#!/bin/bash

# script will compile user solution if needed

function print_help()
{
    cat <<EOF
Usage `basename $0`:

 -h | --help           prints this help and exits
 -s | --solution       path to user solution
 -l | --lang           string value which means solution programming language
 -o | --optimizations  custom optimizations for compiler
EOF
}

E_INTERNAL_ERROR=1
E_NO_SOLUTION=2
E_UNKNOWN_LANGUAGE=3

# parse input data using GNU getopt library
options=$(getopt -o hs:l:o --longoptions solution:,lang:,help,optimizations -- "$@")

while true ; do
    case "$1" in
	-h|--help) print_help; exit 0 ;;
	-s|--solution) solution="$2"; shift 2 ;;
	-l|--lang) lang="$2"; shift 2 ;;
	-o|--optimizations) $optimizations="$2";;
	*) echo "Internal error!" > &2; exit $E_INTERNAL_ERROR;
    esac
done

# if solution doesn't exist - exit with error
if [ ! -e $solution ]
    echo "Specified solution does not exist!" > &2
    exit $E_NO_SOLUTION;
fi

compile_string=""
directory=`dirname $solution`

# extract just filename without extension
program_name=`basename $solution`
program_name=${program_name%.*}
program_path="$directory/$program_name"

optimizations=${optimizations-""}

case $lang in
    "c")compile_string="gcc -o $program_path $optimizations $solution";;
    "cpp")compile_string="g++ -o $program_path $optimizations $solution";;
    "pas")compile_string="fpc -o$program_path $optimizations $solution";;
    "java");;
    "ruby");;
    "py");;
    "perl");;
    "php");;
    # TODO get correct sbcl options here
    "lisp")compile_string="sbcl --script=$solution";;
    *) echo "Such language is not supported!" > &2; exit $E_UNKNOWN_LANGUAGE;;
esac

result=$($compile_string)


