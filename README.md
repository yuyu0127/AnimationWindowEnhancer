# Animation Window Enhancer

This package adds functionality to display curve and color previews in Unity's built-in Animation Window DopeSheet.

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

## Installation

1. Open the Package Manager from **Window > Package Manager**  
2. Click on the "+" button and select **Add package from git URL**  
3. Enter the following URL: `https://github.com/yuyu0127/AnimationWindowEnhancer.git`

Alternatively, open `Packages/manifest.json` and add the following to the `dependencies` block:

```json
{
    "dependencies": {
        "com.yuyu.animationwindowenhancer": "https://github.com/yuyu0127/AnimationWindowEnhancer.git"
    }
}
```

## Features

### Curve and Gradient Preview

Displays previews of curves and gradients in the DopeSheet.

<img width="784" alt="curve and color preview" src="https://github.com/user-attachments/assets/6f932182-42e7-4f53-b2e1-79368a8d75a3" />

### Show Labels

Displays labels on curves in both the DopeSheet and the Curves Editor.

<img width="784" alt="label" src="https://github.com/user-attachments/assets/0afa4529-4585-4075-8a84-106ef91bd798" />

## Preferences

<img width="598" alt="preferences" src="https://github.com/user-attachments/assets/d498b292-bd8a-465a-b15d-8a469bdd4044" />

### Curve
- Default Heatmap: The heatmap used for curves without an override.
- Heatmap Overrides: Overrides the heatmap for specific curves based on property name.
- Resolution: The number of points per frame used to draw the curve.

### Label
- Color: The color of the label on each dope line.
- Font Size: The font size of the label on each dope line.

### Others
- Color Band Height: The height of the color band on dope lines for color properties.
- Parent Dope Line Color: The color of the dope line that has child lines.
