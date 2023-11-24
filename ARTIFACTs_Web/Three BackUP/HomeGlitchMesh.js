<script type="importmap">
{
  "imports": {
    "three": "https://unpkg.com/three@0.156.1/build/three.module.js",
    "three/addons/": "https://unpkg.com/three@0.156.1/examples/jsm/"
  }
}
</script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/simplex-noise/2.4.0/simplex-noise.min.js"></script>
<script type="module">
import * as THREE from 'https://unpkg.com/three@0.156.1/build/three.module.js';
import { EffectComposer } from 'https://unpkg.com/three@0.156.1/examples/jsm/postprocessing/EffectComposer.js';
import { RenderPass } from 'https://unpkg.com/three@0.156.1/examples/jsm/postprocessing/RenderPass.js';
import { GlitchPass } from 'https://unpkg.com/three@0.156.1/examples/jsm/postprocessing/GlitchPass.js';

let scene, camera, renderer, mesh, composer, container, mouseX = 0, mouseY = 0, meshes = [], noise;

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

    // Creare un Array di 4 Mesh con Colori Diversi
    const colors = [0xff0000, 0x00ff00, 0x0000ff, 0xffff00]; // Rosso, Verde, Blu, Giallo

    colors.forEach((color, index) => {
        const material = new THREE.MeshBasicMaterial({ color });
        mesh = new THREE.Mesh(new THREE.BoxGeometry(1, 1, 1), material);
        
        // Posizionamento Random
        mesh.position.x = (Math.random() - 0.5) * 10;
        mesh.position.y = (Math.random() - 0.5) * 10;
    
        // Dimensioni Random 16:9
        let width = Math.random() + 0.5;
        let height = (width * 9) / 16;
        mesh.scale.set(width, height, 1);

        // Rotazione Random
        mesh.rotation.z = getRandomRotation();

    
        scene.add(mesh);
        meshes.push(mesh);
    });
    
    // Composer for post-processing effects
    composer = new EffectComposer(renderer);
    composer.addPass(new RenderPass(scene, camera));
    composer.addPass(new GlitchPass());

    // Camera Position
    camera.position.z = 5;

    // Handle Window Resize
    window.addEventListener('resize', onWindowResize);

    window.addEventListener('mousemove', (event) => {
        mouseX = (event.clientX / window.innerWidth) * 2 - 1;
        mouseY = - (event.clientY / window.innerHeight) * 2 + 1;
    });

    noise = new SimplexNoise();
    scheduleRandomUpdate();
}

function animate() {
    requestAnimationFrame(animate);

    // 3. Cambiare Forma delle Mesh
    meshes.forEach((mesh) => {
        const positions = mesh.geometry.attributes.position.array;
        for (let i = 0; i < positions.length; i += 3) {
            positions[i + 2] += noise.noise2D(positions[i] + Date.now() * 0.0005, positions[i + 1] + Date.now() * 0.0005) * 0.1;
        }
        mesh.geometry.attributes.position.needsUpdate = true; // Segnala che il buffer ha bisogno di un aggiornamento
    });

    meshes.forEach((mesh, index) => {
        let speed = 0.05; // Puoi regolare la velocità qui
        mesh.position.y -= speed; // Muove la mesh verso il basso
        mesh.position.x += mouseX * speed; // Questo orienterà la mesh in base alla posizione del mouse
    });

    // 4. Movimento delle Mesh con il Mouse
    meshes.forEach((mesh, index) => {
        let offsetMultiplier = 0.1;  // Puoi cambiare questo valore per controllare quanto le mesh si spostano
        mesh.position.x += mouseX * offsetMultiplier;
        mesh.position.y += mouseY * offsetMultiplier;
    });
    composer.render();
}

function getRandomRotation() {
    return Math.random() > 0.5 ? 0 : Math.PI / 2;
}

function scheduleRandomUpdate() {
    let randomTime = Math.random() * 5000 + 1000; // Ad esempio, tra 1 e 6 secondi
    setTimeout(() => {
        updateMeshes();
        scheduleRandomUpdate(); // Programma un altro aggiornamento dopo questo
    }, randomTime);
}

function updateMeshes() {
    meshes.forEach(mesh => {
        // Aggiornamento della posizione
        mesh.position.x = (Math.random() - 0.5) * 10;
        mesh.position.y = (Math.random() - 0.5) * 10;
        mesh.position.z = (Math.random() - 0.5) * 10;
        
        // Aggiornamento della rotazione
        mesh.rotation.z = getRandomRotation();
    });
}
container.addEventListener('click', updateMeshes);


function onWindowResize() {
    camera.aspect = container.clientWidth / container.clientHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(container.clientWidth, container.clientHeight);
    composer.setSize(container.clientWidth, container.clientHeight);
}
</script>