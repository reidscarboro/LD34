using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour {

    public enum GameState {
        INITIAL,
        REQUEST,
        PRE_CLIPPING,
        CLIPPING,
        REACTION,
        SCORE,
        TRANSITION
    }

    public GameState gameState;

    public TreeController requestTree;
    public TreeController treeController;
    public ClippersController clippersController;
    public CameraController cameraController;
    public ScoreDisplayController scoreDisplayController;

    public TextMesh currentScore;

    public GameObject customerPrefab;

    public int distanceBetweenCustomers = 8;
    public int numberOfCustomers = 6;

    public float threshold_threeStar = 0.5f;
    public float threshold_twoStar = 0.75f;
    public float threshold_oneStar = 1.0f;
    public float threshold_zeroStar = 1.5f;

    private int seed;
    public int numberOfTreeVertices = 36;
    public float treeVariance = 1.2f;
    public float treeRadius = 1;

    public int clipInterval = 5; //drop a new vertex every n frames
    private int frameCounter = 0;

    private Vector2 previousVertex;
    private List<Vector2> clipVerticies;

    private bool isInitial = true;

    private int timer_initial = 50;
    private int timer_request = 100;
    private int timer_preClipping = 50;
    private int timer_reaction = 25;
    private int timer_score = 75;
    private int timer_transition = 50;

    public int score = 0;
    public int failures = 0;
    private int lastScore = 0;

    private float maxHorizontalDistance = 4.5f;
    private float maxVerticalDistance = 3.5f;

    private List<GameObject> customers;
    private GameObject currentCustomer;

    public List<GameObject> dotMarkers = new List<GameObject>();
    public GameObject dotMarker;

    public LineRenderer lineRenderer;

    void Start() {
        gameState = GameState.INITIAL;
        cameraController.updatePosition(gameState);
        initializeNewCustomers();
        SimplePool.Preload(dotMarker, 200);
    }

	void FixedUpdate() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }


        frameCounter++;
        if (gameState == GameState.INITIAL) {
            if ((frameCounter > timer_initial && !isInitial) || frameCounter > 100) {
                isInitial = false;
                updateGameState(GameState.REQUEST);
            } else {
                for (int i = 0; i < customers.Count; i++) {
                    customers[i].transform.position = Vector3.Lerp(customers[i].transform.position, new Vector3(0, 0, i * distanceBetweenCustomers), 0.1f);
                }
            }
        } else if (gameState == GameState.REQUEST) {
            if (frameCounter > timer_request) {
                updateGameState(GameState.PRE_CLIPPING);
            }
        } else if (gameState == GameState.PRE_CLIPPING) {
            if (frameCounter > timer_preClipping) {
                updateGameState(GameState.CLIPPING);
            }
        } else if (gameState == GameState.CLIPPING) {
            clippersController.move();
            if (frameCounter % clipInterval == 0) { 
                logNewVertex();
            }
        } else if (gameState == GameState.REACTION) {
            if (frameCounter > timer_reaction) {
                updateGameState(GameState.SCORE);
            }
        } else if (gameState == GameState.SCORE) {
            if (frameCounter > timer_score) {
                updateGameState(GameState.TRANSITION);
            }
        } else if (gameState == GameState.TRANSITION) {
            if (frameCounter > timer_transition) {
                updateGameState(GameState.INITIAL);
            } else {
                currentCustomer.transform.position = Vector3.Lerp(currentCustomer.transform.position, new Vector3(-25, 0, 0), 0.1f);
            }
        }
    }

    public void initializeNewCustomers() {
        customers = new List<GameObject>();
        for (int i = 1; i <= numberOfCustomers; i++) {
            GameObject newCustomer = (GameObject) Instantiate(customerPrefab, new Vector3(0, 0, distanceBetweenCustomers * i), Quaternion.identity);
            customers.Add(newCustomer);
        }
    }

    public void initializeCurrentCustomer() {
        currentCustomer = customers[0];
        treeController = currentCustomer.GetComponentInChildren<TreeController>();
        treeController.setVertices(PolygonUtil.getNewPolygon(numberOfTreeVertices, treeVariance, treeRadius, seed, false));
        treeController.updatePolygon();
    }

    public void enableTreeController() {
        treeController.enable();
    }

    public void newTreeRequest() {
        requestTree.setVertices(PolygonUtil.getNewPolygon(numberOfTreeVertices, treeVariance, treeRadius, seed, true));
        requestTree.updatePolygon();
    }

    public void enableRequestDisplay() {
        requestTree.gameObject.SetActive(true);
    }
    
    public void disableRequestDisplay() {
        requestTree.gameObject.SetActive(false);
    }

    public void cycleCustomers() {
        Destroy(currentCustomer);
        customers.RemoveAt(0);
        GameObject newCustomer = (GameObject)Instantiate(customerPrefab, new Vector3(0, 0, distanceBetweenCustomers * numberOfCustomers), Quaternion.identity);
        customers.Add(newCustomer);
    }

    public void logNewVertex() {
        clipVerticies.Add(clippersController.getPosition());
        dotMarkers.Add(SimplePool.Spawn(dotMarker, clippersController.getPosition(), Quaternion.identity));
        checkCutoutComplete();
        if (
            clippersController.getPosition().y > maxVerticalDistance ||
            clippersController.getPosition().y < -maxVerticalDistance ||
            clippersController.getPosition().x > maxHorizontalDistance ||
            clippersController.getPosition().x < -maxHorizontalDistance) {
            finalizeCutout();
        }
    }

    public void clearDotMarkers() {
        foreach (GameObject dotMarker in dotMarkers) {
            SimplePool.Despawn(dotMarker);
        }
        dotMarkers.Clear();
    }

    public void drawLines() {
        Vector3[] linePositions = PolygonUtil.toVector3(requestTree.getVerticies());

        //we have to update the last position to align with the first for some reason
        linePositions[linePositions.Length-1] = linePositions[0];

        lineRenderer.SetVertexCount(linePositions.Length);
        lineRenderer.SetPositions(linePositions);
        lineRenderer.gameObject.SetActive(true);
    }

    public void hideLines() {
        lineRenderer.gameObject.SetActive(false);
    }

    public int checkScore() {
        int deltaScore = -1;
        float differenceArea = PolygonUtil.getDifferenceArea(requestTree.getVerticies(), treeController.getVerticies(), 1000);

        if (differenceArea < threshold_threeStar) {
            deltaScore = 3;
        } else if (differenceArea < threshold_twoStar) {
            deltaScore = 2;
        } else if (differenceArea < threshold_oneStar) {
            deltaScore = 1;
        } else if (differenceArea < threshold_zeroStar) {
            deltaScore = 0;
        }

        if (deltaScore >= 0) {
            score += deltaScore;
        } else {
            failures++;
        }

        return deltaScore;
    }

    public void updateFace() {
        treeController.GetComponentInParent<CustomerController>().updateFace(lastScore);
    }

    public void playScore() {
        SoundController.playScore(lastScore);
        if (lastScore < 0)
            Camera.main.GetComponent<AudioSource>().volume = 0;
    }

    public void incrementDifficulty() {
        //clippersController.speed += 0.0025f;
        //clippersController.turnAngle += 0.25f;
        Time.timeScale += 0.2f;
        Camera.main.GetComponent<AudioSource>().pitch += 0.02f;
    }

    public void endGame() {
        Time.timeScale = 1;
        int best = PlayerPrefs.GetInt("best");
        int yesterday = score;
        PlayerPrefs.SetInt("yesterday", yesterday);
        if (yesterday > best) {
            PlayerPrefs.SetInt("best", yesterday);
        }
        SceneManager.LoadScene("intro");
    }

    public void checkCutoutComplete() {
        if (clipVerticies.Count > 2) {
            int removeBeforeIndex = 0;
            bool intersectionFound = false;
            for (int i = 1; i < clipVerticies.Count - 2; i++) {
                if (LineUtil.doIntersect(previousVertex, clippersController.getPosition(), clipVerticies[i - 1], clipVerticies[i])) {
                    intersectionFound = true;
                    removeBeforeIndex = i;
                    break;
                }
            }
            if (intersectionFound) {
                clipVerticies.RemoveRange(0, removeBeforeIndex);
                clipVerticies.RemoveAt(clipVerticies.Count - 1);
                finalizeCutout();
            } else {
                previousVertex = clippersController.getPosition();
            }
        }
    }

    public void finalizeCutout() {
        clipVerticies.Reverse();
        treeController.setVertices(PolygonUtil.getIntersection(treeController.getVerticies(), PolygonUtil.toVector2(clipVerticies), 500));
        treeController.updatePolygon();
        updateGameState(GameState.REACTION);
    }

    public void updateGameState(GameState _gameState) {
        gameState = _gameState;
        cameraController.updatePosition(gameState);
        frameCounter = 0;

        if (gameState == GameState.INITIAL) {
            if (failures >= 3) {
                endGame();
            } else {
                cycleCustomers();
                if (lastScore > 0) incrementDifficulty();
            }
        } else if (gameState == GameState.REQUEST) {
            seed = Random.Range(0, 100000);
            initializeCurrentCustomer();
            newTreeRequest();
            requestTree.gameObject.SetActive(true);
            enableRequestDisplay();
        } else if (gameState == GameState.PRE_CLIPPING) {
            enableTreeController();
            drawLines();
            clippersController.enable();
        } else if (gameState == GameState.CLIPPING) {
            disableRequestDisplay();
            clipVerticies = new List<Vector2>();
            previousVertex = clippersController.getPosition();
        } else if (gameState == GameState.REACTION) {
            hideLines();
            clearDotMarkers();
            lastScore = checkScore();
            updateFace();
            playScore();
            clippersController.disable();
        } else if (gameState == GameState.SCORE) {
            if (lastScore > -1) {
                scoreDisplayController.showScore(lastScore);
            } else {
                scoreDisplayController.showFailures(failures);
            }
            currentScore.text = score.ToString();
        } else if (gameState == GameState.TRANSITION){
            scoreDisplayController.clear();
            if (failures < 3) Camera.main.GetComponent<AudioSource>().volume = 1;
        }
    }
}
