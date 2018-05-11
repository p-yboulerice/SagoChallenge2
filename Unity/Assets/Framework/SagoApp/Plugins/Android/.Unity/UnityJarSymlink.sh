#!/bin/sh

PARENT_FOLDER="/Applications/Unity-"
SUB_FOLDER="/PlaybackEngines/AndroidPlayer/Variations/mono/Release/Classes/classes.jar"
UNITY_STRING="Unity"
JAR_NAME="classes.jar"

function to_lower() {
    local str="$1"
    local output=$(tr '[A-Z]' '[a-z]'<<<"${str}")
    echo $output
}

function str_len() {
    local var=${#1}
    echo "$var"
}

function get_unity_versions() {
    local i=0
    for unity_folder_location in $(find /Applications -maxdepth 1 -name "Unity*"); do
        IFS='-' read -ra folder_name_tokens <<< "$unity_folder_location"
        
        local tokenLength=${#folder_name_tokens[@]}
        for (( j=1; j<tokenLength; j++ )); do
            if [ $j -eq 1 ]
            then
                VERSIONS[$i]+="${folder_name_tokens[$j]}"
            else
                VERSIONS[$i]+="-${folder_name_tokens[$j]}"
            fi
        done
        
        ((i++))
    done
}

get_unity_versions

INDEX=1
for ver in "${VERSIONS[@]}"; do
    echo "[$INDEX] Unity Version: ${ver}"
    ((INDEX++))
done

#LENGTH=$( str_len ${VERSION_ARG} )

re='^-?[0-9]+([.][0-9]+)?$'

while [ true ]; do
    read -p "Choose an option or type version: " VERSION_ARG
    
    if ! [[ $VERSION_ARG =~ $re ]]; then
        TARGET_PATH="${PARENT_FOLDER}${VERSION_ARG}${SUB_FOLDER}"
        break;
    else
        if [ "$VERSION_ARG" -ge 1 ] && [ "$VERSION_ARG" -lt "${#VERSIONS[@]}" ]; then
            ((VERSION_ARG--))
            TARGET_PATH="${PARENT_FOLDER}${VERSIONS[VERSION_ARG]}${SUB_FOLDER}"
            break
        else
            echo "Invalid argument!"
        fi
    fi
done

ln -sf "$TARGET_PATH" "$JAR_NAME"

echo "Symlink has been switched to: $TARGET_PATH"