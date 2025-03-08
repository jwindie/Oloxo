package Core;

import processing.core.PApplet;
import processing.core.PConstants;

public class DrawSettings implements Cloneable   {
    public static DrawSettings lastAppliedSettings = new DrawSettings();

    public static final int STROKE_CAP_SQUARE = PConstants.SQUARE;
    public static final int STROKE_CAP_ROUND = PConstants.ROUND;
    public static final int STROKE_CAP_PROJECT = PConstants.PROJECT;

    public int fill;
    public int stroke;
    public int strokeCap;
    public float strokeWeight;

    public boolean useFill;
    public boolean useStroke;

    public DrawSettings(){}
    public DrawSettings(DrawSettings other) {
        this.fill         = other.fill;
        this.stroke       = other.stroke;
        this.strokeCap    = other.strokeCap;
        this.strokeWeight = other.strokeWeight;
        this.useFill      = other.useFill;
        this.useStroke    = other.useStroke;
    }

    //#region GETTERS
    public int getFill() {return fill;}
    public int getStroke() {return stroke;}
    public int getStrokeCap() {return strokeCap;}
    public float getStrokeWeight() {return strokeWeight;}
    public boolean getUseFill () { return useFill;}
    public boolean getUseStroke () { return useStroke;}
    //#endregion
    
    //#region SETTERS
    public void setFill (int fill) {
        this.fill = fill;
    }
    public void setStroke(int stroke) {
        this.stroke = stroke;
    }  public void setStrokeCap(int strokeCap) {
        this.strokeCap = strokeCap;
    }
    public void setStrokeWeight (float strokeWeight) {
        this.strokeWeight = strokeWeight;
    }
    public void setUseFill (boolean useFill) {
        this.useFill = useFill;
    }   
    public void setUseStroke(boolean useStroke) {
        this.useStroke = useStroke;
    }
    //#endregion

    /**Pushes the settings to the PApplet. */
    public void applyDrawSettings(PApplet target) {
        if (useFill) {
            target.fill (fill);
        }else target.noFill();

        if (useStroke && strokeWeight > 0) {
            target.stroke(stroke);
            target.strokeWeight(strokeWeight);
            target.strokeCap(strokeCap);
        }else target.noStroke();

        copy(this, lastAppliedSettings);
    }

    @Override
    public DrawSettings clone() {
        try {
            DrawSettings ds = (DrawSettings) super.clone();
            // You can also apply custom logic for the new object, if needed
            ds.setFill(fill);
            ds.setStroke(stroke);
            ds.setStrokeCap(strokeCap);
            ds.setStrokeWeight(strokeWeight);
            ds.setUseFill(useFill);
            ds.setUseStroke(useStroke);
            return ds;
        } catch (CloneNotSupportedException e) {
            throw new AssertionError();  // Can never happen because we implement Cloneable
        }
    }

    public static void copy(DrawSettings source, DrawSettings target) {
        target.setFill(source.getFill());
        target.setStroke(source.getStroke());
        target.setStrokeCap  (source.getStrokeCap());
        target.setStrokeWeight (source.getStrokeWeight());
        target.setUseFill (source.getUseFill());
        target.setUseStroke (source.getUseStroke());
    }

    @Override
    public String toString() {
        return 
            String.format ("Fill: %b, f(%d)\n", useFill, fill) +
            String.format ("Stroke %b, s(%d), sw(%f), sc(%d)", useStroke, stroke, strokeWeight, strokeCap)
        ;
    }
}