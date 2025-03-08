package Obsolete;
// package Quark;

// import processing.core.PApplet;
// import processing.core.PVector;

// public class Line extends AbstractShape  {  

//     PVector startPoint, endPoint;
//     public boolean drawBounds;

//     public Line(PVector startPoint, PVector endPoint) {
//         this.startPoint = startPoint;
//         this.endPoint = endPoint;
//     }
//     public void setDrawBounds(boolean state) {
//         this.drawBounds = state;
//     }
//     public void setstartPoint (PVector startPoint) {
//         this.startPoint = startPoint;
//     }
//     public void setEnd (PVector endPoint) {
//         this.endPoint = endPoint;
//     }
//     public void setStrokeCap (StrokeCap strokeCap) {
//         this.strokeCap = strokeCap;
//     }

//     @Override
//     public void draw(PApplet target) {
//         target.strokeWeight(strokeWeight);
//         target.stroke(stroke);
//         target.strokeCap(getStrokeCapInt());
//         target.line (
//             startPoint.x,
//             startPoint.y,
//             endPoint.x,
//             endPoint.y
//         );
//         target.strokeCap(StrokeCap.DEFAULT.getValue());


//         if (drawBounds)  {
//             //figure out the boundary with quick math
//             float hs  = strokeWeight/2f;
//             float xMin = Math.min(startPoint.x, endPoint.x) - hs;
//             float xMax = Math.max(startPoint.x, endPoint.x) + hs;

//             float yMin = Math.min(startPoint.y, endPoint.y) - hs;
//             float yMax = Math.max(startPoint.y, endPoint.y) + hs;
//             drawBoundingBox(target, xMin, yMin, xMax - xMin, yMax - yMin);
//         }
//     }
// }