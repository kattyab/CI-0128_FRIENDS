export default function (p) {
  let sec, min, hr;

  p.setup = () => {
    let canvas = p.createCanvas(400, 400);
    canvas.parent('clock-container');
    canvas.style('background', 'transparent');
    p.angleMode(p.DEGREES);
  };

  p.draw = () => {
    p.clear();
    p.translate(200, 200);
    p.rotate(-90);

    updateTime();

    // Clock circle
    p.noFill();
    p.stroke('#003C63');
    p.strokeWeight(5);
    p.ellipse(0, 0, 300, 300);

    // Hour hand
    drawHand(p.map(hr % 12 + min / 60, 0, 12, 0, 360), 60, 8);

    // Minute hand
    drawHand(p.map(min + sec / 60, 0, 60, 0, 360), 90, 6);

    // Second hand
    // drawHand(p.map(sec, 0, 60, 0, 360), 110, 2);

    p.strokeWeight(8);
    p.point(0, 0);
  };

  function updateTime() {
    sec = p.second();
    min = p.minute();
    hr = p.hour();
  }

  function drawHand(angle, length, weight) {
    p.push();
    p.rotate(angle);
    p.stroke('#003C63');
    p.strokeWeight(weight);
    p.line(0, 0, length + 20, 0);
    p.pop();
  }
}
