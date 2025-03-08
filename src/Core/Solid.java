package Core;

import processing.core.PApplet;
import processing.core.PVector;

public class Solid extends AbstractShape  {  
    public enum Shape {
        RECTANGLE,
        ELLIPSE
    }

    PVector position, size, anchor, anchorOffset;
    float[] corners;
    public Shape shape;
    public boolean drawBounds;

    public Solid(Shape shape) {
        this.shape = shape;
        this.size = new PVector(50,50);
        this.position = new PVector();
        this.anchor = new PVector();
        this.anchorOffset = new PVector();
        applyDefaultDrawSettings();
    }
    public Solid (Shape shape,PVector position, PVector size) {
        this.shape = shape;
        this.size = size;
        this.position = position;
        this.anchor = new PVector();
        this.anchorOffset = new PVector();
        applyDefaultDrawSettings();
    }
    public Solid (Shape shape,PVector position, PVector size, float ul, float ur, float lr, float ll) {
        this.shape = shape;
        this.size = size;
        this.position = position;
        this.anchor = new PVector();
        this.anchorOffset = new PVector();
        setCorners(ul, ur, lr, ll);
        applyDefaultDrawSettings();
    }
    public Solid (Shape shape, PVector position, PVector size, float[] corners) {
        this.shape = shape;
        this.size = size;
        this.position = position;
        this.anchor = new PVector();
        this.anchorOffset = new PVector();
        if (corners != null && corners.length  > 0) {
            setCorners(
                corners.length > 0 ? corners[0] : 0,
                corners.length > 1 ? corners[1] : 0,
                corners.length > 2 ? corners[2] : 0,
                corners.length > 3 ? corners[3] : 0
            );
        }
        applyDefaultDrawSettings();
    }

    public PVector getAnchoredPosition() {
        return PVector.add(anchorOffset, anchor);
    }
    public float[] getCorners() {
        if (corners == null) return new float[] {0,0,0,0};
        else return corners.clone();
    }
    public Shape getShape() { return shape; }

    public void setAnchor (PVector anchor) {
        //clamp the values
        if (anchor.x < 0) anchor.x = 0;
        if (anchor.x > 1) anchor.x = 1;

        if (anchor.y < 0) anchor.y = 0;
        if (anchor.y > 1) anchor.y = 1;

        //apply the values
        this.anchor = anchor;
        updateAnchorOffset();
    }
    public  void setCorners (float a) {
        if (a == 0) return;
        corners = new float[] {a, a, a, a};
    }
    public void setCorners (float ul, float ur, float ll, float lr) {
        if (ul == 0 && ur == 0 && ll == 9 && lr == 0) return;
        corners = new float[] {ul, ur, ll, lr};
    }   
    public void setDrawBounds(boolean state) {
        this.drawBounds = state;
    }
    public void setPosition (PVector position) {
        this.position = position;
    }
    public void setShape(Shape shape) {
        this.shape = shape;
    }
    public void setSize (PVector size ) {
        this.size = size;
        updateAnchorOffset();
    }

    protected void overrideDrawSettings(PApplet target) {}

    @Override
    public void draw(PApplet target) {
        if (!ignoreDrawSettings) settings.applyDrawSettings(target);

        overrideDrawSettings(target); //if there are settingsoverrides we will inject them here

        switch (shape) {
            case Shape.RECTANGLE:
                target.rect (
                    position.x + anchorOffset.x, 
                    position.y + anchorOffset.y,
                    size.x, 
                    size.y,
                    corners == null ? 0 : corners[0],
                    corners == null ? 0 : corners[1],
                    corners == null ? 0 : corners[2],
                    corners == null ? 0 : corners[3]
                ); 
            break;
            case Shape.ELLIPSE:
            target.ellipse (
                    position.x + anchorOffset.x, 
                    position.y + anchorOffset.y,
                    size.x, 
                    size.y
                ); 
            break;
        }

        if (drawBounds)  {
            float hs = settings.getStrokeWeight() / 2f;
            drawBoundingBox(
                target, 
                position.x + anchorOffset.x - hs,
                position.y + anchorOffset.y - hs,
                size.x  + settings.getStrokeWeight(),
                size.y + settings.getStrokeWeight()
            );
        }
    }

    protected void updateAnchorOffset() {
        anchorOffset = new PVector(
            -size.x * anchor.x,
            -size.y * anchor.y
        );
    }

    @Override
    protected void applyDefaultDrawSettings() {
        settings.setFill(Utils.DefaultColors.WHITE);
        settings.setUseFill(true);
    }
}