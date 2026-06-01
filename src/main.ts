const canvasElement = document.getElementById('game');
if (!(canvasElement instanceof HTMLCanvasElement)) {
  throw new Error('Game canvas not found');
}

const canvas = canvasElement;
const ctx = canvas.getContext('2d');
if (!ctx) {
  throw new Error('2D context unavailable');
}

const context = ctx;

function drawFrame(time: number): void {
  context.fillStyle = '#16213e';
  context.fillRect(0, 0, canvas.width, canvas.height);

  context.fillStyle = '#e94560';
  context.font = '24px system-ui, sans-serif';
  context.textAlign = 'center';
  context.fillText('Stone Game', canvas.width / 2, canvas.height / 2 - 12);
  context.font = '14px system-ui, sans-serif';
  context.fillStyle = '#a0a0c0';
  context.fillText('Ready to build', canvas.width / 2, canvas.height / 2 + 20);
  context.fillText(`t=${Math.floor(time)}ms`, canvas.width / 2, canvas.height / 2 + 44);

  requestAnimationFrame(drawFrame);
}

requestAnimationFrame(drawFrame);
