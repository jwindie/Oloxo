package Quark;

import processing.core.PApplet;
import processing.core.PVector;

public class Button extends Solid  {  
    public enum HoverMode {
        FILL,
        STROKE,
        FILLSTROKE
    }

    private static int idCount = 0;
    int hoveredFill, selectedFill, disabledFill;
    // float hoveredStrokeWeight;
    // HoverMode hoverMode = HoverMode.STROKE;
    
    boolean hovered;
    boolean disabled;
    // boolean selected;

    /**Does the select fill take priority over the hover fill?*/
    // boolean selectFillPriority = true;
    int id;
    
    //constructors
    public Button(Shape shape) {
        super(shape);
        id = idCount++;
    }
    public Button (Shape shape, PVector position, PVector size) {
        super(shape, position, size);
        id = idCount++;
    }
    public Button (Shape shape,PVector position, PVector size, float ul, float ur, float lr, float ll) {
        super (shape, position, size, ul, ur, ll, lr);
        id = idCount++;
    }
    public Button (Shape shape, PVector position, PVector size, float[] corners) {
        super (shape, position, size, corners);
        id = idCount++;
    }

    //getters
    // HoverMode getHoverMode() {return hoverMode;}
    boolean getHovered() { return hovered;}
    // boolean getSelected() { return selected;}
    boolean getDisabled() { return disabled;}
    // boolean getSelectFillPriority() {return selectFillPriority;}
    int getId () {return id;}
    int getHoveredFill() { return hoveredFill;}
    // int getSelectedFill() { return selectedFill;}
    // int getDisabledFill() { return disabledFill;}
    // float getHoveredStrokeWeight() { return hoveredStrokeWeight;}

    //setters
    // void setHoverMode(HoverMode hoverMode) {
    //     this.hoverMode = hoverMode;
    // }
    public void setHoveredFill (int hoveredFill) {
        this.hoveredFill = hoveredFill;
    }
    public void setDisabledFill(int disabledFill) {
        this.disabledFill = disabledFill;
    }
    public void setDisabled(boolean disabled) {
        this.disabled = disabled;
    }


    @Override
    protected void overrideDrawSettings(PApplet target) {

        // if (selectFillPriority) {
        //    if (hovered) onApplyHoverDrawSettings(target);
        //    if (selected) target.fill(selectedFill);
        // }else {
            // if (selected) target.fill(selectedFill);
            if (hovered) onApplyHoverDrawSettings(target);
        // }
        if (disabled) target.fill (disabledFill);
    }

    private void onApplyHoverDrawSettings(PApplet target) {
        // switch (hoverMode) {
        //     case HoverMode.FILL:
                target.fill (hoveredFill);
        //     break;
        //     case HoverMode.STROKE:
        //         target.stroke(hoveredFill);
        //         target.strokeWeight(hoveredStrokeWeight);
        //     break;
        //     case HoverMode.FILLSTROKE:
        //         target.strokeWeight(hoveredStrokeWeight);
        //         target.fill (hoveredFill);
        //     break;
        // }
    }
  
    public void onUpdate (float x, float y) {
        if (disabled) {
            hovered = false; return;
        }
        hovered = 
            x >= position.x + anchorOffset.x && x <= position.x + size.x + anchorOffset.x &&
            y >= position.y + anchorOffset.y && y <= position.y + size.y + anchorOffset.y 
        ;
    }
    
    // boolean click(int mouseButton) {
    //   return selected = !selected;
    // }
}