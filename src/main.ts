const canvas = document.getElementById('game');
if (!(canvas instanceof HTMLCanvasElement)) {
  throw new Error('Game canvas not found');
}

const ctx = canvas.getContext('2d');
if (!ctx) {
  throw new Error('2D context unavailable');
}

const gameContext = ctx;

function drawFrame(time: number): void {
  gameContext.fillStyle = '#16213e';
  gameContext.fillRect(0, 0, canvas.width, canvas.height);

  gameContext.fillStyle = '#e94560';
  gameContext.font = '24px system-ui, sans-serif';
  gameContext.textAlign = 'center';
  gameContext.fillText('Stone Game', canvas.width / 2, canvas.height / 2 - 12);
  gameContext.font = '14px system-ui, sans-serif';
  gameContext.fillStyle = '#a0a0c0';
  gameContext.fillText('Ready to build', canvas.width / 2, canvas.height / 2 + 20);
  gameContext.fillText(`t=${Math.floor(time)}ms`, canvas.width / 2, canvas.height / 2 + 44);

  requestAnimationFrame(drawFrame);
}

requestAnimationFrame(drawFrame);
