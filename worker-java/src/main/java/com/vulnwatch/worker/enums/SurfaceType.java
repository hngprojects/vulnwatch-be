package com.vulnwatch.worker.enums;

import io.swagger.v3.oas.annotations.media.Schema;

/**
 * Surface types that can be scanned by different scanners.
 * Each scanner returns results for one surface type.
 */
@Schema(description = "Type of security surface being scanned")
public enum SurfaceType {

    @Schema(description = "DNS records and configurations (SPF, DMARC, DNSSEC, etc.)")
    DNS,

    @Schema(description = "SSL/TLS certificates, protocols, and cipher suites")
    SSL,

    @Schema(description = "HTTP security headers (HSTS, CSP, X-Frame-Options, etc.)")
    HTTP_HEADERS,

    @Schema(description = "Dependency vulnerabilities (npm, maven, pip packages)")
    DEPENDENCY,

    @Schema(description = "Hardcoded secrets, API keys, and credentials")
    SECRETS,

}