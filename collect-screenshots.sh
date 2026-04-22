#!/bin/bash
# Collects screenshots from the running Sample app using MauiDevFlow.
# Prerequisites:
#   1. Build & run the Sample app in Debug on iOS simulator
#   2. Ensure maui-devflow CLI is installed: dotnet tool install --global Redth.MauiDevFlow.CLI
#
# Usage: ./collect-screenshots.sh [platform]
#   platform: ios (default), maccatalyst, android

set -e

PLATFORM="${1:-ios}"
OUTPUT_DIR="$(cd "$(dirname "$0")" && pwd)/assets"
CLI="maui-devflow"
SETTLE=1.5

echo "Collecting screenshots to: $OUTPUT_DIR"
echo "Platform: $PLATFORM"
echo ""

if ! $CLI MAUI status -p "$PLATFORM" --no-json 2>/dev/null; then
    echo "ERROR: Cannot connect to MauiDevFlow agent."
    echo "Make sure the Sample app is running in Debug mode."
    exit 1
fi

nav() {
    echo "Navigating to //$1..."
    $CLI MAUI navigate "//$1" -p "$PLATFORM" --no-json
    sleep "$SETTLE"
}

capture() {
    echo "  -> Capturing $1"
    $CLI MAUI screenshot --output "$OUTPUT_DIR/$1" --overwrite --max-width 400 -p "$PLATFORM" --no-json
}

find_and_tap() {
    local selector="$1"
    local id
    id=$($CLI MAUI query --selector "$selector" -p "$PLATFORM" --json 2>/dev/null \
         | python3 -c "import sys,json; data=json.load(sys.stdin); print(data[0]['id'])" 2>/dev/null || echo "")
    if [ -n "$id" ]; then
        $CLI MAUI tap "$id" -p "$PLATFORM" --no-json
        sleep "$SETTLE"
        return 0
    fi
    return 1
}

set_prop() {
    local selector="$1"
    local prop="$2"
    local value="$3"
    local id
    id=$($CLI MAUI query --selector "$selector" -p "$PLATFORM" --json 2>/dev/null \
         | python3 -c "import sys,json; data=json.load(sys.stdin); print(data[0]['id'])" 2>/dev/null || echo "")
    if [ -n "$id" ]; then
        $CLI MAUI set-property "$id" "$prop" "$value" -p "$PLATFORM" --no-json
        sleep "$SETTLE"
        return 0
    fi
    return 1
}

echo "=== Scheduler ==="
nav "calendar"
capture "scheduler1.png"
nav "agenda"
capture "scheduler2.png"
nav "calendarlist"
capture "scheduler3.png"

echo ""
echo "=== SheetView ==="
nav "sheet"
capture "sheet1.png"
if find_and_tap "Button[Text='Open Sheet']"; then
    capture "sheet2.png"
fi

nav "minimizedsheet"
capture "sheet3.png"
if find_and_tap "Button[Text='Open Music Player']"; then
    capture "sheet4.png"
fi

nav "topsheet"
capture "sheet5.png"
if find_and_tap "Button[Text='Open Weather']"; then
    capture "sheet6.png"
fi

echo ""
echo "=== Pills ==="
nav "pills"
capture "pills.png"

echo ""
echo "=== Security Pin ==="
nav "securitypin"
capture "securitypin.png"

echo ""
echo "=== FAB & FabMenu ==="
nav "fab"
capture "fab-closed.png"
if set_prop "FabMenu" "IsOpen" "true"; then
    capture "fab-open.png"
fi

echo ""
echo "=== TableView ==="
nav "basic"
capture "tableview-basic.png"
nav "dynamic"
capture "tableview-dynamic.png"
nav "dragsort"
capture "tableview-dragsort.png"
nav "picker"
capture "tableview-picker.png"
nav "styling"
capture "tableview-styling.png"

echo ""
echo "=== Image Viewer ==="
nav "imageviewer"
capture "imageviewer1.png"
# Find the large tappable image (has tap gesture, >100px wide)
IMG_ID=$($CLI MAUI query --selector "Image" -p "$PLATFORM" --json 2>/dev/null \
    | python3 -c "
import sys, json
elements = json.load(sys.stdin)
for e in elements:
    if 'tap' in e.get('gestures', []) and e.get('bounds', {}).get('width', 0) > 100:
        print(e['id']); break
" 2>/dev/null || echo "")
if [ -n "$IMG_ID" ]; then
    $CLI MAUI tap "$IMG_ID" -p "$PLATFORM" --no-json
    sleep "$SETTLE"
    capture "imageviewer2.png"
fi

echo ""
echo "=== Image Editor ==="
nav "imageeditor"
sleep 2
capture "imageeditor1.png"
if find_and_tap "Button[Text='Crop']"; then
    capture "imageeditor2.png"
    find_and_tap "Button[Text='Cancel']"
fi

echo ""
echo "=== Color Picker ==="
nav "colorpicker"
capture "colorpicker1.png"

echo ""
echo "=== Chat ==="
nav "chat"
capture "chat1.png"

echo ""
echo "=== Markdown ==="
nav "markdownview"
capture "markdown-view.png"
nav "markdowneditor"
capture "markdown-editor.png"

echo ""
echo "=== Mermaid Diagrams ==="
nav "flowchart"
capture "mermaid-flowchart.png"
nav "editor"
capture "mermaid-editor.png"
nav "themes"
capture "mermaid-themes.png"
nav "subgraphs"
capture "mermaid-subgraphs.png"

echo ""
echo "========================================="
echo "Screenshot collection complete!"
echo "Files saved to: $OUTPUT_DIR"
echo "========================================="
