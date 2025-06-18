# Tech

1. Git, push your code
2. Github Actions => Workflow
3. EC2 Launch template, launches EC2 intances
4. Auto Scale Group, maintains scalability and automation of EC2 instances
5. AWS Code Build, builds source code
6. EC2 => Target groups

# Configuration: 

Github Repo: Create variable to be used in workflow file as they are not secret

```bash
GitHub Repo → Settings → Secrets and Variables → Actions → Variables
```

Replace ? with appropriate values

```bash
AWS_BUCKET=?
AWS_REGION=?
AWS_CODE_BUILD_PROJECT_NAME=?
```


Github Repo: Create variable to be used in workflow file

```bash
GitHub Repo → Settings → Secrets and Variables → Actions → Secrets
```

Replace ? with appropriate values

```bash
AWS_ACCESS_KEY_ID=?
AWS_SECRET_ACCESS_KEY=?
```


## IAM

1. Create a user for Github. 
   Download ACCESS_KEY_ID and ACCESS_SECRET that will be configured in Github secrets.
   Give following access: 
   - Amazon  S3 access
   - autoscaling:StartInstanceRefresh to your auto scaling group
   - codebuild:StartBuild, codebuild:BatchGetBuilds for your CodeBuild

2. Create a role for launch template.
   Give following access:
   - AmazonEC2RoleforAWSCodeDeploy
   - s3:ListBucket for S3 bucket 
   - s3:GetObject for S3 bucket
    

   Launch template configuration => UserData

```bash
#!/bin/bash
exec > >(tee /var/log/user-data.log | logger -t user-data -s 2>/dev/console) 2>&1

# Install required packages
yum update -y
yum install -y wget tar

# Install .NET 8 SDK
sudo curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
sudo chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --install-dir /opt/dotnet
sudo ln -s /opt/dotnet/dotnet /usr/bin/dotnet

# Create application directory
mkdir -p /var/www/myapp
cd /var/www/myapp

# Copy published app from S3 (make sure files exist)
aws s3 cp s3://autoscale-test/publish/ . --recursive

# Start the ASP.NET Core app on port 5000
nohup dotnet ASWebApp.dll --urls "http://0.0.0.0:5000" > app.log 2>&1 &
```
	
## Flow

1. Code is push to Github main branch
2. Github Actions executes, zips the code and puts in S3=>source folder as `app.zip`
	- Zip source code into app.zip
	- Copies app.zip to S3 bucket => source folder
	- Triggers CodeBuild to build the code and put into same S3 bucket => artifacts folder
	- Refreshes auto scaling group
