# ZephyrRelations

## How To Work With Multiple Github Accounts on a single Machine

### Step1: Create SSH keys for all accounts

```bash
ssh-keygen -t rsa -C "david.yaari@icloud.com" -f "david-yaari"
ssh-keygen -t rsa -C "alephtechhouse@gmail.com" -f "alephtechhouse"
```

### Step 2: Add SSH keys to SSH Agent

```bash
ssh-add --apple-use-keychain ~/.ssh/david-yaari
ssh-add --apple-use-keychain ~/.ssh/alephtechhouse
```

### Step 3: Add SSH public key to the Github

```bash
pbcopy < ~/.ssh/david-yaari.pub
pbcopy < ~/.ssh/alephtechhouse.pub
```

### Step 4: Create a Config File and Make Host Entries

```bash
touch config
open config

#david-yaari account
Host github.com-david-yaari
     HostName github.com
     User git
     IdentityFile ~/.ssh/david-yaari

#alephtechhouse account
Host github.com-alephtechhouse
     HostName github.com
     User git
     IdentityFile ~/.ssh/alephtechhouse
```

### Step 5: Cloning GitHub repositories using different accounts

```bash
git config user.email "alephtechhouse@gmail.com"
git config user.name "AlephTechHouse"

git remote add origin git@github.com-alephtechhouse:ZephyrRelations.git

git clone git@github.com-alephtechhouse:alephtechhouse/ZephyrRelations.git

git config user.email "david.yaari@icloud.com"
git config user.name "David Yaari"
```
