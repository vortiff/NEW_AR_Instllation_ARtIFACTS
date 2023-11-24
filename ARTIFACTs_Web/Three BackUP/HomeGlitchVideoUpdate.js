<script type="importmap">
{
  "imports": {
    "three": "https://unpkg.com/three@0.156.1/build/three.module.js",
    "three/addons/": "https://unpkg.com/three@0.156.1/examples/jsm/"
  }
}
</script>
<script type="module">
import * as THREE from 'https://unpkg.com/three@0.156.1/build/three.module.js';
import { EffectComposer } from 'https://unpkg.com/three@0.156.1/examples/jsm/postprocessing/EffectComposer.js';
import { RenderPass } from 'https://unpkg.com/three@0.156.1/examples/jsm/postprocessing/RenderPass.js';
import { GlitchPass } from 'https://unpkg.com/three@0.156.1/examples/jsm/postprocessing/GlitchPass.js';

let scene, camera, renderer, composer, container;

init();
animate();

function init() {
    // Scene, Camera, and Renderer Setup
    scene = new THREE.Scene();
    camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    renderer = new THREE.WebGLRenderer();
    container = document.getElementById("myCanvas");
    renderer.setPixelRatio(window.devicePixelRatio);
    container.appendChild(renderer.domElement);
    renderer.setSize(container.clientWidth, container.clientHeight);
    
    // Composer for post-processing effects
    composer = new EffectComposer(renderer);
    composer.addPass(new RenderPass(scene, camera));
    composer.addPass(new GlitchPass());

    // Camera Position
    camera.position.z = 5;

    // Handle Window Resize
    window.addEventListener('resize', onWindowResize);
    
    // Update video at random intervals
    scheduleRandomVideoUpdate();
}

function animate() {
    requestAnimationFrame(animate);
    composer.render();
}

function scheduleRandomVideoUpdate() {
    let randomTime = Math.random() * (90000 - 30000) + 30000; // Between 30 and 90 seconds
    setTimeout(() => {
        updateVideoSource();
        scheduleRandomVideoUpdate();
    }, randomTime);
}

function updateVideoSource() {
    const videoElement = document.getElementById("245a2deb-33ed-e59c-ab59-d26527b5452d-video");
    if (videoElement) {
        // Replace with the URL of the new video source you want to use
        videoElement.src = "URL_OF_YOUR_NEW_VIDEO";
    }
}

function onWindowResize() {
    camera.aspect = container.clientWidth / container.clientHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(container.clientWidth, container.clientHeight);
    composer.setSize(container.clientWidth, container.clientHeight);
}
</script>
