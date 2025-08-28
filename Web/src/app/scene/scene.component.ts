import { Component, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import { Point } from '../../models/point';
import { Torus } from '../../models/torus';

@Component({
  selector: 'app-scene',
  standalone: true,
  template: '<div class="scene-container"></div>',
  styles: [`
    .scene-container {
      width: 100%;
      height: 100vh;
      position: absolute;
      top: 0;
      left: 0;
    }
  `]
})
export class SceneComponent implements AfterViewInit {
  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private controls!: OrbitControls;
  private animationId!: number;
  private axesHelper!: THREE.AxesHelper;
  private raycaster!: THREE.Raycaster;
  private mouse!: THREE.Vector2;

  // Добавляем массив для хранения кликабельных объектов
  private clickableObjects: THREE.Object3D[] = [];

  constructor(private el: ElementRef) { }

  ngAfterViewInit(): void {
    this.initThree();
    //this.setupEventListeners();
    this.animate();
  }

  private initThree(): void {
    // Создаем сцену
    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0xffffff);

    // Создаем камеру
    this.camera = new THREE.PerspectiveCamera(
      75,
      window.innerWidth / window.innerHeight,
      0.1,
      1000
    );
    this.camera.position.z = 100;

    // Создаем рендерер
    this.renderer = new THREE.WebGLRenderer({ antialias: true });
    this.renderer.setSize(window.innerWidth, window.innerHeight);
    this.renderer.setPixelRatio(window.devicePixelRatio);

    const container = this.el.nativeElement.querySelector('.scene-container');
    container.appendChild(this.renderer.domElement);

    this.controls = new OrbitControls(this.camera, this.renderer.domElement);

    this.camera.updateProjectionMatrix();

    // Обработка изменения размера окна
    window.addEventListener('resize', () => this.onWindowResize());

    this.axesHelper = new THREE.AxesHelper(1000);
    this.scene.add(this.axesHelper);

    // Raycaster для определения кликов
    this.raycaster = new THREE.Raycaster();
    this.mouse = new THREE.Vector2();
  }

  private onWindowResize(): void {
    this.camera.aspect = window.innerWidth / window.innerHeight;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(window.innerWidth, window.innerHeight);
  }

  private animate(): void {
    this.animationId = requestAnimationFrame(() => this.animate());

    this.controls.update(); // Обновляем контролы
    this.renderer.render(this.scene, this.camera);
  }

  public clearScene(cubeEdge: number): void {
    while (this.scene.children.length > 0) {
      this.scene.remove(this.scene.children[0]);
    }
    this.controls.reset();
    this.camera.position.set(0, 0, cubeEdge * 1.8);
    this.scene.add(this.axesHelper);
  }

  public createWireframeCube(cubeEdge: number): void {
    const halfEdge = cubeEdge / 2;

    // Вершины куба
    const vertices = [
      [-halfEdge, -halfEdge, -halfEdge], // 0
      [halfEdge, -halfEdge, -halfEdge], // 1
      [halfEdge, halfEdge, -halfEdge], // 2
      [-halfEdge, halfEdge, -halfEdge], // 3
      [-halfEdge, -halfEdge, halfEdge], // 4
      [halfEdge, -halfEdge, halfEdge], // 5
      [halfEdge, halfEdge, halfEdge], // 6
      [-halfEdge, halfEdge, halfEdge], // 7
    ];

    // Линии, соединяющие вершины (каждая пара индексов формирует линию)
    const edges = [
      0, 1, 1, 2, 2, 3, 3, 0,  // Нижняя грань
      4, 5, 5, 6, 6, 7, 7, 4,  // Верхняя грань
      0, 4, 1, 5, 2, 6, 3, 7   // Соединение нижней и верхней грани
    ];

    // Преобразование вершин в формат, используемый для BufferGeometry
    const cubeVertices: number[] = [];
    edges.forEach(index => {
      cubeVertices.push(...vertices[index]);
    });

    // Создаем BufferGeometry и добавляем линии
    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(cubeVertices, 3));

    // Материал для линий
    const material = new THREE.LineBasicMaterial({ color: 0xff0000 });

    // Создаем объект линии
    const cubeWireframe = new THREE.LineSegments(geometry, material);

    this.scene.add(cubeWireframe);
  }

  public vizualizeTori(torusList: Torus[], cubeEdge: number): void {
    this.clearScene(cubeEdge);

    const g = new THREE.BoxGeometry(cubeEdge, cubeEdge, cubeEdge);
    const m = new THREE.MeshBasicMaterial({ color: 0xff0000, wireframe: false, transparent: true, opacity: 0.2 });
    this.scene.add(new THREE.Mesh(g, m));
    this.createWireframeCube(cubeEdge);

    var counter = 0;
    torusList.forEach(t => {
      const geometry = new THREE.TorusGeometry(t.majorRadius, t.minorRadius, 8, 16);
      const material = new THREE.MeshBasicMaterial({ color: t.majorRadius * 0xffffff, wireframe: true });
      const torus = new THREE.Mesh(geometry, material);

      torus.position.set(t.center.x, t.center.y, t.center.z);
      torus.rotation.set(t.rotation.x, t.rotation.y, t.rotation.z);
      //torus.userData = {
      //  id: counter,
      //  center: {
      //    x: t.center.x,
      //    y: t.center.y,
      //    z: t.center.z,
      //  },
      //  rotation: {
      //    x: t.rotation.x,
      //    y: t.rotation.y,
      //    z: t.rotation.z,
      //  },
      //  majorRadius: t.majorRadius,
      //  minorRadius: t.minorRadius
      //};
      //this.clickableObjects.push(torus);

      this.scene.add(torus);

      counter = counter + 1;
    });
  }

  private setupEventListeners(): void {
    // Обработка кликов
    this.renderer.domElement.addEventListener('click', this.onMouseClick.bind(this));

    //window.addEventListener('resize', this.onWindowResize.bind(this));
  }

  private onMouseClick(event: MouseEvent): void {
    // Получаем координаты мыши относительно canvas
    const rect = this.renderer.domElement.getBoundingClientRect();
    this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

    // Обновляем луч
    this.raycaster.setFromCamera(this.mouse, this.camera);

    // Проверяем пересечения только с кликабельными объектами
    const intersects = this.raycaster.intersectObjects(this.clickableObjects);

    if (intersects.length > 0) {
      const clickedObject = intersects[0].object;
      console.log(clickedObject.userData);
    }
  }
}
