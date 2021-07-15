git branch --merged origin/main \
| grep -vE 'main'\
| xargs git branch -d 