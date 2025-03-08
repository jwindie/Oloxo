package Core;

import processing.core.PApplet;

public abstract class AbstractShape extends Entity {
    protected DrawSettings settings = new DrawSettings();

    public boolean ignoreDrawSettings = false;   

    public DrawSettings getDrawSettings() {return settings;}
    public DrawSettings copySettings() {return settings.clone();}
    

    public boolean getIgnoreDrawSettings() {return ignoreDrawSettings;
    }

    public void setIgnoreDrawSettings(boolean ignoreDrawSettings) {
        this.ignoreDrawSettings = ignoreDrawSettings;
    }
    public void setShapeSettings(DrawSettings settings) {
        this.settings = settings;
    }

    /**Draw a bounding box around the object. Will by default be magenta */
    protected void drawBoundingBox (PApplet target, float x, float y, float w, float h) {
        target.noFill();
        target.stroke( target.color(255,0,255));
        target.strokeWeight(1);
        target.rect (x, y, w, h);
    }

    public abstract void draw(PApplet target);
}