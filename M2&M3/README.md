# Milestone 2 & 3: Core AI System (Pose Estimation & Context-Aware Alert) 🧠🚨

## 🎯 Objective
This milestone represents the "Brain" and "Eyes" of the research project. It combines two critical phases:
1. Using **MediaPipe Pose** to extract 33 biological landmarks from the human body in real-time.
2. Applying **Spatial Context-Awareness** and **Dynamic Time Thresholds** to those coordinates to accurately detect falls and stroke risks, ultimately triggering a privacy-preserving emergency alert via Telegram.

## 🛠️ Key Tasks
### Part A: Pose Estimation
- [ ] Initialize video streaming and frame processing using **OpenCV**.
- [ ] Integrate **MediaPipe Pose** to detect and draw the human skeleton.
- [ ] Calculate the falling velocity and the spine angle relative to the ground.

### Part B: Context-Aware Logic & Alert
- [ ] Define **Spatial Zones (ROI)** (e.g., Bed vs. Floor) to eliminate false positives during normal activities like sleeping.
- [ ] Implement **Micro-movement Tracking** to detect absolute immobility over dynamic time thresholds (e.g., 5 seconds for the floor vs. 15 minutes for the bed).
- [ ] Integrate the Telegram API using the `requests` library to send automated alerts.
- [ ] Automatically blur or blackout sensitive background images before dispatching the emergency alert to protect user privacy.

## 📝 Note
By combining Pose Estimation with Context-Aware Logic, this milestone directly solves the "False Positive" issue common in traditional camera-based fall detection systems.
