import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { RoomEnvironment } from "https://cdn.jsdelivr.net/gh/mrdoob/three.js@80be9a58b20db73cba4ba9f70f8eee9b646a661a/examples/jsm/environments/RoomEnvironment.js";
import { GLTFLoader } from "three/addons/loaders/GLTFLoader.js";
import { DRACOLoader } from "three/addons/loaders/DRACOLoader.js";
import { KTX2Loader } from "three/addons/loaders/KTX2Loader.js";
import { EffectComposer } from "three/addons/postprocessing/EffectComposer.js";
import { RenderPass } from "three/addons/postprocessing/RenderPass.js";
//import { GlitchPass } from "three/addons/postprocessing/GlitchPass.js";
document.addEventListener("DOMContentLoaded", function () {
  console.log('DOM fully loaded and parsed'); // Dovrebbe apparire per primo
  // Crea una funzione per aggiornare la larghezza della barra del loader
  function updateLoaderBar(percentage) {
    console.log(`Updating loader bar to ${percentage}%`); // Log per il debugging
    const loaderBar = document.getElementById('LoaderBar');
    if (loaderBar) {
      loaderBar.style.width = `${percentage}%`;
    } else {
    console.error('LoaderBar element not found'); // Log per il debugging
    }
  }
  // Crea una funzione per nascondere il loader una volta che il caricamento è completo
  function hideLoader() {
    console.log('Hiding loader'); // Log per il debugging
    const loaderWrap = document.getElementById('LoaderWrap');
    if (loaderWrap) {
      loaderWrap.style.display = 'none';
    } else {
      console.error('LoaderWrap element not found'); // Log per il debugging
    }
  }
  const container = document.getElementById("container");
  let mixer;
  const clock = new THREE.Clock();
  let model;
  // Renderer
  const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
  renderer.setPixelRatio(window.devicePixelRatio);
  renderer.setSize(container.clientWidth, container.clientHeight);
  container.appendChild(renderer.domElement);
  // Generatore di PMREM e Scena
  const pmremGenerator = new THREE.PMREMGenerator(renderer);
  const scene = new THREE.Scene();
  scene.background = null;
  scene.environment = pmremGenerator.fromScene(
    new RoomEnvironment(renderer),
    0.04
  ).texture;
  // Camera
  const camera = new THREE.PerspectiveCamera(40, window.innerWidth / window.innerHeight, 1, 100);
  camera.position.set(5, 2, 8);
  // Controlli della camera
  window.controls = new OrbitControls(camera, renderer.domElement);
  controls.target.set(0, 0.5, 0);
  controls.update();
  controls.enablePan = false;
  controls.enableDamping = true;
  // Limita la rotazione verticale
  controls.minPolarAngle = Math.PI / -10;
  controls.maxPolarAngle = Math.PI / 2;
  // Limita la rotazione orizzontale
  controls.minAzimuthAngle = -Math.PI / 20;
  controls.maxAzimuthAngle = Math.PI / 2;
  // Configurazione dell'EffectComposer
  const composer = new EffectComposer(renderer);
  const renderPass = new RenderPass(scene, camera);
  composer.addPass(renderPass);
  /* Aggiungi GlitchPass
  const glitchPass = new GlitchPass();
  glitchPass.goWild = false; // Disabilita le transizioni più intense
  glitchPass.curF = 0; // Modifica la frequenza delle transizioni
  composer.addPass(glitchPass);

  composer.addPass(glitchPass);
  */
  // Loader DRACO,KTX2 e GLTF
  const dracoLoader = new DRACOLoader();
  dracoLoader.setDecoderPath(
    "https://cdn.jsdelivr.net/gh/mrdoob/three.js@master/examples/jsm/libs/draco/gltf/"
  );
  const ktx2Loader = new KTX2Loader();
  ktx2Loader.setTranscoderPath(
    "https://cdn.jsdelivr.net/gh/mrdoob/three.js@master/examples/js/libs/basis/"
  );
  ktx2Loader.detectSupport(renderer);
  const loader = new GLTFLoader();
  loader.setDRACOLoader(dracoLoader);
  loader.setKTX2Loader(ktx2Loader);
  loader.load(
    "https://cdn.jsdelivr.net/gh/vortiff/NEW_AR_Instllation_ARtIFACTS@main/ARtiFACTs_Assets/glb/NewMapAnimationTest4.glb",
    function (gltf) {
      model = gltf.scene;
      model.position.set(1, 0, 1);
      model.scale.set(0.01, 0.01, 0.01);
      scene.add(model);
      model.rotation.x = THREE.MathUtils.degToRad(17.188);
      model.rotation.y = THREE.MathUtils.degToRad(15);
      model.rotation.z = THREE.MathUtils.degToRad(0);
      // Traversa i nodi del modello
      model.traverse((child) => {
        if (child.isMesh) {
          console.log(
            `Name: ${child.name}, Material: ${child.material.name}, Map: ${child.material.map}`
          );
        }
      });
      animate();
    },
    undefined,
    handleError
  );
  const isMobile = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
  if (isMobile) {
    container.addEventListener("touchstart", onDocumentMouseDown, false);
  } else {
    container.addEventListener("click", onDocumentMouseDown, false);
  }

  const raycaster = new THREE.Raycaster(); // Definisci raycaster
  let originalVertices = {};
  // Creare una nuova istanza di SimplexNoise
  const simplex = new SimplexNoise();
  function animateMeshes(meshNames) {
    // Ottieni il tempo corrente
    const time = performance.now() * 0.001;
    model.traverse((child) => {
      if (child.isMesh && meshNames.includes(child.name)) {
        // Store original vertices if not stored yet
        if (!originalVertices[child.name]) {
          originalVertices[child.name] =
            child.geometry.attributes.position.array.slice();
        }
        let vertices = child.geometry.attributes.position.array;
        let original = originalVertices[child.name];
        for (let i = 0; i < vertices.length; i += 3) {
          // Utilizza Simplex Noise per creare un movimento più fluido e naturale
          const scale = 0.8; // Regola la scala del rumore
          const speed = 0.4; // Regola la velocità dell'animazione
          const glitchAmount = 0.9; // Regola l'intensità dell'effetto glitch
          vertices[i] =
            original[i] +
            simplex.noise3D(
              original[i] * scale,
              original[i + 1] * scale,
              time * speed
            ) *
              glitchAmount; // x
          vertices[i + 1] =
            original[i + 1] +
            simplex.noise3D(
              original[i + 1] * scale,
              original[i + 2] * scale,
              time * speed
            ) *
              glitchAmount; // y
          vertices[i + 2] =
            original[i + 2] +
            simplex.noise3D(
              original[i] * scale,
              original[i + 2] * scale,
              time * speed
            ) *
              glitchAmount; // z
        }
        child.geometry.attributes.position.needsUpdate = true;
      }
    });
  }
  function animate() {
    requestAnimationFrame(animate);
    model.rotation.y += 0.001;
    animateMeshes([
      "Empty_interaction_1",
      "Empty_interaction_2",
      "Empty_interaction_3",
      "Empty_interaction_4",
      "Empty_interaction_5",
      "Empty_interaction_6",
    ]);
    composer.render();
  }
  window.onresize = function () {
    camera.aspect = container.clientWidth / container.clientHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(container.clientWidth, container.clientHeight);
  };
  let wrapDocElement = document.querySelector(
    '[data-w-id="aaf856a2-deeb-f0de-0eec-bcaced7c4287"]'
  );
  if (container) {
    container.addEventListener("click", onDocumentMouseDown, false);
  } else {
    console.error("'container' element not found!");
  }
  if (wrapDocElement) {
    wrapDocElement.addEventListener("click", function () {
      console.log("wrap_doc was programmatically clicked!");
    });
  } else {
    console.error("'wrap_doc' element not found using data-w-id!");
  }
  function onDocumentMouseDown(event) {
    event.preventDefault(); // Previene comportamenti di default
    // Normalizza le coordinate del mouse
    let mouse = new THREE.Vector2();
    // Calcola la posizione del tocco/mouse e aggiungi una tolleranza
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = -(event.clientY / window.innerHeight) * 2 + 1;
    // Aggiungi una tolleranza per dispositivi touch
    const touchTolerance = 0.5; // Aumenta o diminuisci questo valore in base alla sensibilità desiderata
    mouse.x += touchTolerance;
    mouse.y += touchTolerance;

    let raycaster = new THREE.Raycaster();
    raycaster.setFromCamera(mouse, camera);
    // Trova oggetti intersecati
    let intersects = raycaster.intersectObjects(scene.children, true);
    // Verifica quante intersezioni hai
    if (intersects.length > 0) {
      const objectMap = {
        Empty_interaction_1: "aaf856a2-deeb-f0de-0eec-bcaced7c4287",
        Empty_interaction_2: "8afb3a77-2f0c-81f5-ebd8-8c2ca1c16322",
        Empty_interaction_3: "8afb3a77-2f0c-81f5-ebd8-8c2ca1c16322",
        Empty_interaction_4: "4f58e33c-832c-f7c1-da2e-b956c92470a7",
        Empty_interaction_5: "958b6c40-8773-8641-d41c-e57a68a953eb",
        Empty_interaction_6: "d6695961-18ae-fda6-a1d9-73b0c881d01a",
      };
      if (intersects.length > 0) {
        /// Se c'è almeno un oggetto intersecato
        console.log("First intersected object: ", intersects[0].object.name);
        // Ottieni il data-w-id corrispondente all'oggetto intersecato
        const dataWId = objectMap[intersects[0].object.name];
        if (dataWId) {
          // Se l'oggetto corretto è intersecato, simuliamo un click sull'elemento 'wrapDocElement'
          const wrapDocElement = document.querySelector(
            `[data-w-id='${dataWId}']`
          );
          if (wrapDocElement) {
            console.log(`Element with [data-w-id='${dataWId}'] clicked!`);
            setTimeout(() => {
              wrapDocElement.click();
            }, 100);
          } else {
            console.error(`Element with [data-w-id='${dataWId}'] not found!`);
          }
        }
      }
    }
  }
});
function handleError(err) {
  console.error(err);
}


// Utilizza LoadingManager per tenere traccia del progresso del download
const manager = new THREE.LoadingManager();
manager.onStart = function (url, itemsLoaded, itemsTotal) {
  // Inizia mostrando il loader e impostando la barra di caricamento a 0%
  updateLoaderBar(0);
};

manager.onLoad = function () {
  // Nascondi il loader una volta che tutti gli asset sono stati caricati
  hideLoader();
};

manager.onProgress = function (url, itemsLoaded, itemsTotal) {
  // Aggiorna il loader bar in base al progresso
  const progress = (itemsLoaded / itemsTotal) * 100;
  updateLoaderBar(progress);
};

manager.onError = function (url) {
  console.error('There was an error loading ' + url);
};

const loader = new GLTFLoader(manager);
