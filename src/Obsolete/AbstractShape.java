package Griddom;
// package Quark;

// import Quark.Core.Entity;
// import processing.core.PApplet;

// public abstract class AbstractShape extends Entity {
//     public int fill;
//     public int stroke = 0;
//     public float strokeWeight = 1;
//     public StrokeCap strokeCap = StrokeCap.DEFAULT;

//     public AbstractShape(){
//         fill = Integer.decode("#FFFFFF");
//     }
//     public AbstractShape(int fill) {
//         this.fill = fill;
//     }
//     public AbstractShape(int fill, int stroke) {
//         this.fill = fill;
//         this.stroke = stroke;
//     }
//     public AbstractShape(int fill, int stroke, float strokeWeight) {
//         this.fill = fill;
//         this.stroke = stroke;
//         this.strokeWeight = strokeWeight;
//     }

//     public int getFill() {return fill;}
//     public int getStroke() {return stroke;}
//     public float getStrokeWeight() {return strokeWeight;}
//     public StrokeCap getStrokeCap () {return strokeCap;}
//     public int getStrokeCapInt () {return strokeCap.getValue();}

//     public void setFill (int fill) {
//         this.fill = fill;
//     }
//     public void setStroke(int stroke) {
//         this.stroke = stroke;
//     }
//     public void setStrokeWeight (float strokeWeight) {
//         this.strokeWeight = strokeWeight;
//     }

//     public void pushDisplaySettings(PApplet app) {
//         if (fill <= 0) app.fill (fill); else app.noFill();
//         if (strokeWeight <= 0)  {
//             app.stroke (stroke); 
//             app.strokeWeight(strokeWeight);
//         } 
//         else app.noStroke();
//     }
//     public abstract void draw (PApplet app);

//     protected void drawBoundingBox (PApplet target, float x, float y, float w, float h) {
//         target.noFill();
//         target.stroke( target.color(255,0,255));
//         target.strokeWeight(1);
//         target.rect (x, y, w, h);
//     }
// }
