import tkinter as tk
from tkinter import messagebox
from pathlib import Path
import subprocess


# ---------------------------------
# Repository search
# ---------------------------------

def find_git_repo():

    current = Path(__file__).resolve().parent

    while current != current.parent:

        if (current / ".git").exists():
            return current

        current = current.parent

    return None


# ---------------------------------
#   Git command
# ---------------------------------

def run_git(repo, args):

    result = subprocess.run(
        ["git"] + args,
        cwd=repo,
        capture_output=True,
        text=True
    )

    if result.returncode != 0:
        raise Exception(result.stderr)

    return result.stdout


# ---------------------------------
# see if changes 
# ---------------------------------

def has_changes(repo):

    result = subprocess.run(
        ["git", "status", "--porcelain"],
        cwd=repo,
        capture_output=True,
        text=True
    )

    return result.stdout.strip() != ""


# ---------------------------------
# Commit + Push
# ---------------------------------

def commit_push():

    message = txt_message.get().strip()

    if message == "":
        messagebox.showwarning(
            "Git Helper",
            "Please enter a commit message."
        )
        return

    try:

        run_git(repo, ["add", "."])

        run_git(
            repo,
            ["commit", "-m", message]
        )

        run_git(
            repo,
            ["push"]
        )

        messagebox.showinfo(
            "Git Helper",
            "Commit & Push completed successfully."
        )

        root.destroy()

    except Exception as ex:

        messagebox.showerror(
            "Git Error",
            str(ex)
        )


# ---------------------------------
# search for repository
# ---------------------------------

repo = find_git_repo()

if repo is None:

    messagebox.showerror(
        "Git Helper",
        "No Git repository found."
    )

    exit()

if not has_changes(repo):
    exit()


# ---------------------------------
# window creation
# ---------------------------------

root = tk.Tk()

root.title("Git Helper")
root.geometry("500x220")
root.resizable(False, False)

# Project name
project_name = repo.name

lbl_title = tk.Label(
    root,
    text="Git Helper",
    font=("Segoe UI", 16, "bold")
)
lbl_title.pack(pady=10)

lbl_repo = tk.Label(
    root,
    text=f"Repository: {project_name}",
    font=("Segoe UI", 10)
)
lbl_repo.pack()

lbl_status = tk.Label(
    root,
    text="Changes detected.",
    fg="green",
    font=("Segoe UI", 10)
)
lbl_status.pack(pady=5)

lbl_message = tk.Label(
    root,
    text="Commit Message:"
)
lbl_message.pack(anchor="w", padx=20)

txt_message = tk.Entry(
    root,
    width=60
)
txt_message.pack(padx=20, pady=5)

btn_commit = tk.Button(
    root,
    text="Commit & Push",
    width=18,
    command=commit_push
)
btn_commit.pack(pady=20)

txt_message.focus()

root.mainloop()


    