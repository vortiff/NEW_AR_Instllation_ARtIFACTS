<script type="importmap">
{
    "imports": {
        "three": "https://unpkg.com/three@0.156.1/build/three.module.js",
        "three/addons/": "https://unpkg.com/three@0.156.1/examples/jsm/"
    }
}
</script>

<script type="module">
import * as THREE from 'three';
import { EffectComposer } from 'three/addons/postprocessing/EffectComposer.js';
import { RenderPass } from 'three/addons/postprocessing/RenderPass.js';
import { GlitchPass } from 'three/addons/postprocessing/GlitchPass.js';

document.addEventListener("DOMContentLoaded", (event) => {
    let scene, camera, renderer, composer, cube, container;

    function init() {
        scene = new THREE.Scene();
        camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
        container = document.getElementById("myCanvas");
        
        renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(container.clientWidth, container.clientHeight);
        container.appendChild(renderer.domElement); // Assicurati che il canvas sia appeso al container

        composer = new EffectComposer(renderer);
        composer.addPass(new RenderPass(scene, camera));

        const glitchPass = new GlitchPass();
        composer.addPass(glitchPass);

        // Utilizzo dell'elemento video esistente
        const video = document.getElementById('d6067381-cf3a-cfcd-c651-356d8f88fdc6-video');
       
        video.addEventListener('canplay', function() {
            console.log('Video can play!');
        }, false);
        
        video.addEventListener('error', function() {
            console.error('Video error:', video.error);
        }, false);
        const videoTexture = new THREE.VideoTexture(video);
        const videoMaterial = new THREE.MeshBasicMaterial({ map: videoTexture });
    
        let mesh = new THREE.Mesh(new THREE.BoxGeometry(), videoMaterial);
        scene.add(mesh);
    

        camera.position.z = 5;

        window.addEventListener('resize', onWindowResize);
    }

    function animate() {
        requestAnimationFrame(animate);
        composer.render();
    }

    function onWindowResize() {
        camera.aspect = container.clientWidth / container.clientHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(container.clientWidth, container.clientHeight);
        composer.setSize(container.clientWidth, container.clientHeight);
    }

    init();
    animate();
});
</script>
