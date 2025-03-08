package Core;

import Utils.DefaultColors;
import processing.core.PApplet;
import processing.core.PVector;

public class Line extends AbstractShape  {  

    PVector startPoint, endPoint;
    public boolean drawBounds;

    public Line() {
        this.startPoint = new PVector(20,20);
        this.endPoint = new PVector(100,100);
        applyDefaultDrawSettings();
    }
    public Line(PVector startPoint, PVector endPoint) {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        applyDefaultDrawSettings();
    }

    public void setDrawBounds(boolean state) {
        this.drawBounds = state;
    }
    public void setStartPoint (PVector startPoint) {
        this.startPoint = startPoint;
    }
    public void setEndPoint (PVector endPoint) {
        this.endPoint = endPoint;
    }
    public void setPoints(PVector startPoint, PVector endPoint) {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    @Override
    public void draw(PApplet target) {
        settings.applyDrawSettings(target);

        target.line (
            startPoint.x,
            startPoint.y,
            endPoint.x,
            endPoint.y
        );

        if (drawBounds)  {
            //figure out the boundary with quick math
            float hs  = settings.getStrokeWeight()/2f;
            float xMin = Math.min(startPoint.x, endPoint.x) - hs;
            float xMax = Math.max(startPoint.x, endPoint.x) + hs;

            float yMin = Math.min(startPoint.y, endPoint.y) - hs;
            float yMax = Math.max(startPoint.y, endPoint.y) + hs;
            drawBoundingBox(target, xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }

    @Override
    protected void applyDefaultDrawSettings() {
        settings.setStrokeWeight(2);
        settings.setStroke(DefaultColors.WHITE);
        settings.setUseStroke(true);
        settings.setStrokeCap(DrawSettings.STROKE_CAP_ROUND);
    }
}