for remotebranch in $(git branch -r --merged origin/main \
| grep -vE 'main|rc-' \
| sed 's/origin\///g'); do
git push origin --delete $remotebranch
done