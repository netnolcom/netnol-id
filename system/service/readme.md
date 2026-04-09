<section align="center">
    <h2 align="center">
        <spam>Netnol ID</spam>
        <br />
        <spam>
            <sup align="center"><sub><strong>Own your identity, today and tomorrow</strong></sub></sup>
        </spam>
    </h2>
    <div>
        <h3>What is Netnol Identity Service?</h3>
        <p>This service is the central API and manager for Netnol ID. It provides a secure, private, and universal authentication layer using Zero-Knowledge Proofs (ZKP) and Post-Quantum Cryptography.</p>
        <h3>Infrastructure</h3>
        <p>Built with .NET and ASP.NET Core, the system utilizes CockroachDB for distributed SQL and Redis for high-performance caching. It operates as a standalone instance while depending on Netnol.Identity.Core for essential utilities and shared logic. All configuration is managed via .env files or environment variables.</p>
        <h3>Commands</h3>
        <p>Migrations: 
            <br/><code>dotnet-ef migrations add &lt;NAME&gt; -o .\src\Infrastructure\Migrations</code>
            <br/><code>dotnet-ef database update</code>
        </p>
        <p>Execution:
            <br/><code>dotnet watch run // Development environment only.</code>
            <br/><code>dotnet run -c Release // Allows release optimization</code>        
        </p>
        <h3>Deployment</h3>
        <p>Portable via Dockerfile. Loads configuration from environment variables at runtime.</p>
    </div>
    <br />
    <div>
        <p align="center"><sup>Made with 💕 by Netnol</sup></p>
    </div>
</section>
