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
    let scene, camera, renderer, composer, container, video;

    video = document.createElement('video');
    video.crossOrigin = "anonymous";
    video.src = "https://main--tourmaline-bublanina-19d39f.netlify.app/videos/New_Mapskurz_klein_250.m4v";
    video.muted = true;
    video.loop = true;
    video.addEventListener('canplaythrough', onVideoReady, false);

    function init() {
        scene = new THREE.Scene();
        camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
        container = document.getElementById("myCanvas");
        
        renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(container.clientWidth, container.clientHeight);
        container.appendChild(renderer.domElement);

        composer = new EffectComposer(renderer);
        composer.addPass(new RenderPass(scene, camera));
        const glitchPass = new GlitchPass();
        composer.addPass(glitchPass);

        window.addEventListener('resize', onWindowResize);
    }

    function onVideoReady() {
        if(video.readyState < video.HAVE_FUTURE_DATA) {
            console.log("Video non ancora pronto");
            return;
        }
    
        console.log("Video pronto!");
        video.play();
        
        const texture = new THREE.VideoTexture(video);
        texture.minFilter = THREE.LinearFilter;
        texture.magFilter = THREE.LinearFilter;
        texture.format = THREE.RGBFormat;
        
        let mesh = new THREE.Mesh(new THREE.BoxGeometry(), new THREE.MeshBasicMaterial({ map: texture }));
        scene.add(mesh);
    
        camera.position.z = 5;
        
        animate();
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
});

</script>
