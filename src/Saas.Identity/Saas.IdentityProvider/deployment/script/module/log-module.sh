#!/usr/bin/env bash

source "constants.sh"
source "$SCRIPT_MODULE_DIR/colors-module.sh"

function echo-color() {
    local text=""
    local color=""
    local level="info"

    # if input is piped in, assign it into $text
    if [[ -p /dev/stdin ]]; then
        text=$(</dev/stdin)
    # else, use arguments
    else
        text="$1"
        # shift command like argument to next argument
        shift
    fi

    # parse short and long arguments
    while (( $# >= 1 )); do
        case "${1}" in 
            --color | -c)
                    color="${2}"
                    shift
                    ;;
            --level | -l)
                    level="${2}"
                    shift
                    ;;
            --text | -t)
                    text="${2}"
                    shift
                    ;;
            *) # ignore any other arguments
        esac
        # shift command like argument to next argument
        shift
    done

    if test -z "${color}"; then
        case "${level}" in
            "success" | "Success" | "s" )
                color="${GREEN}"
                ;;
            "info" | "Info" | "i" )
                color="${CYAN}"
                ;;
            "message" | "Message" | "m" | "msg" | "Msg")
                color="${WHITE}"
                ;;
            "warn" | "Warn" | "warning" | "Warning" | "w" )
                color="${YELLOW}"
                ;;
            "error" | "Error" | "e"	)
                color="${RED}"
                ;;
            *)
                color="${NC}" # default to no color
        esac
    fi

    # if color is set, set it
    if test -n "${color}"; then
        echo -e -n "${color}" > /dev/tty
    fi

    # print text
    echo -e "${text}" > /dev/tty

    # reset color
    echo -e -n "${NC}" > /dev/tty
}

function log-output() {
    local text=""
    local header=""
    local level="info"
    local color=""

    local is_error=false

    # if input is piped in, assign it into $text
    if [[ -p /dev/stdin ]]; then
        text=$(</dev/stdin)
    # else, use arguments
    else
        text="$1"
        # shift command like argument to next argument
        shift
    fi

    # parse short and long arguments
    while (( $# >= 1 )); do
        case "${1}" in 
            --header | -h)
                    header="${2}"
                    shift
                    ;;
            --level | -l)
                    level="${2}"
                    shift
                    ;;
            --color | -c)
                    color="${2}"
                    shift
                    ;;
            *) echo "Invalid argument ${1} : ${2}" >&2;  exit 1
        esac
        # shift command like argument to next argument
        shift
    done

    case "${level}" in
        "success" | "Success" | "s" )
            ;;
        "info" | "Info" | "i" )
            ;;
        "message" | "Message" | "m" | "msg" | "Msg")
            ;;
        "warn" | "Warn" | "warning" | "Warning" | "w" )
            ;;
        "error" | "Error" | "e"	)
            is_error=true
            ;;
        *)
            # default to info
            level="info"
    esac

    if test -n "${color}"; then

        # if header is set echo it
        if test -n "${header}"; then
            echo
            echo-color \
                --text "### ${header} ###" \
                --color "${color}"
        fi

        echo-color \
            --text "${text}" \
            --color "${color}"

    elif test -n "${level}"; then
        # if header is set echo it
        if test -n "${header}"; then
            echo
            echo-color \
                --text "### ${header} ###" \
                --level "${level}"
        fi
        
        echo-color \
            --text "${text}" \
            --level "${level}"
    fi

    { 
        if test -n "$header"; then
            echo
            echo "### ${header} ###"
            echo "${text}"
        else
            echo "# ${text}"
        fi
    } >> "${LOG_FILE_DIR}/deploy-${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}.log"

    if [[ $is_error == true ]]; then
        exit 1
    fi
}