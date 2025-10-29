let currentAudio=null,currentBtn=null;
function makeAudio(src,vol){const a=new Audio(src);a.loop=true;a.volume=vol??0.3;return a;}
document.addEventListener('click',e=>{const btn=e.target.closest('.btn.play');if(!btn) return;
const card=btn.closest('.card');const src=btn.dataset.src;const vol=card.querySelector('.vol');
if(currentAudio && currentAudio.src.includes(src) && !currentAudio.paused){currentAudio.pause();btn.textContent='▶';btn.classList.remove('pause');currentAudio=null;currentBtn=null;return;}
if(currentAudio){currentAudio.pause();if(currentBtn){currentBtn.textContent='▶';currentBtn.classList.remove('pause');}}
const a=makeAudio(src,parseFloat(vol.value));a.play();currentAudio=a;currentBtn=btn;btn.textContent='⏸';btn.classList.add('pause');
vol.addEventListener('input',()=>{if(a)a.volume=parseFloat(vol.value);},{once:true});});