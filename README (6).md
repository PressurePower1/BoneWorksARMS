# Pony's BW Rig — BONELAB Mod

Removes arm colliders while keeping grabbing intact, replicating the Boneworks rig feel. Quest compatible.

---

## How to get the DLL (no PC needed)

### Step 1 — Create a GitHub account
Go to https://github.com and sign up for a free account if you don't have one.

### Step 2 — Create a new repository
1. Click the **+** button in the top right → **New repository**
2. Name it `BoneworksArms`
3. Set it to **Public**
4. Click **Create repository**

### Step 3 — Upload the files
On the repository page, click **uploading an existing file** and upload ALL of these files keeping the folder structure:
```
BoneworksArms.cs
BoneworksArms.csproj
.github/workflows/build.yml
```
> For the `.github/workflows/build.yml` file — create it manually:
> 1. Click **Create new file**
> 2. Type `.github/workflows/build.yml` as the filename (GitHub will create the folders)
> 3. Paste the contents of `build.yml` in

### Step 4 — Trigger the build
The build starts automatically when you upload files. To check it:
1. Click the **Actions** tab at the top of your repo
2. You should see a workflow run called **Build BoneworksArms Mod**
3. Wait for the green checkmark ✅ (takes about 1–2 minutes)

### Step 5 — Download the DLL
1. Click on the completed workflow run
2. Scroll down to **Artifacts**
3. Click **BoneworksArms** to download a `.zip`
4. Unzip it — inside is `BoneworksArms.dll`

### Step 6 — Install on Quest
Use SideQuest or ADB to push the DLL to your Quest:
```
adb push BoneworksArms.dll /sdcard/Android/data/com.StressLevelZero.BONELAB/files/Mods/
```
Or drag and drop it via SideQuest's file manager to:
```
/sdcard/Android/data/com.StressLevelZero.BONELAB/files/Mods/
```

---

## Requirements
- MelonLoader installed on your Quest BONELAB
- BoneLib installed on your Quest BONELAB
