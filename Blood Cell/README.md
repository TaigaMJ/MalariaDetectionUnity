# 🩺 Malaria Detection in Unity

A real-time AI-powered malaria diagnosis system built with **TensorFlow → ONNX → Unity Barracuda**.  
Trained on blood cell images to classify **Parasitized (Infected)** vs. **Uninfected** cells.

![Malaria Detection UI](https://via.placeholder.com/600x300?text=Screenshot+of+Unity+UI+Here) <!-- Replace with actual screenshot later -->

## 📌 Features
- ✅ Binary classification of malaria-infected blood cells
- ✅ Trained on [Malaria Cell Images Dataset (Kaggle)](https://www.kaggle.com/datasets/iarunava/cell-images-for-detecting-malaria)
- ✅ Real-time inference in Unity using **Barracuda**
- ✅ Medical-grade UI with doctor-themed interface
- ✅ Cross-platform: runs on **Windows, macOS, Android, iOS**

## 🧠 Model Details
- **Architecture**: Custom CNN (3 Conv layers + Global Average Pooling)
- **Input**: 128×128 RGB image
- **Output**: Probability of infection (`> 0.5 = Infected`)
- **Accuracy**: ~92% validation accuracy
- **Format**: ONNX (compatible with Unity Barracuda)

## 🚀 How to Use

### 1. Open Project
- Requires **Unity 2021.3 LTS or newer**
- Open the `Blood Cell` folder as a Unity project

### 2. Assign Model (if not auto-loaded)
- Select `MalariaManager` GameObject
- Drag `malaria_cnn_corrected.nn` into the **Model Asset** field

### 3. Run Inference
- Click the **"🔍 Analyze Cell"** button
- View results in the **Diagnosis Panel**:
  - 🔴 **INFECTED** (confidence %)
  - 🟢 **HEALTHY** (confidence %)

### 4. Customize
- Replace `DoctorBackground.jpg` with your own medical background
- Add more test images to `Assets/UI/Images/`

## 📂 Project Structure